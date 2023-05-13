//-----------------------------------------------------------------------
// <copyright file="VrModeController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.UI;

/// <summary>
/// Turns VR mode on and off.
/// </summary>
public class FirstPersonController : MonoBehaviour
{
	// Field of view value to be used when the scene is not in VR mode. In case
	// XR isn't initialized on startup, this value could be taken from the main
	// camera and stored.
	private const float _defaultFieldOfView = 60.0f;

	// Main camera from the scene.
	private Camera _Camera;

	// Mouse movement for PC Users
	public float MouseSensitivity = 2.0f;
	private float MouseX = 0.0f, MouseY = 0.0f;

	// Player movement properties
	CharacterController _Controller;
	public float MovementSpeed = 10.0f;

	// Raycasting to replace the default cardboard script
	private GameObject _CurrentGazedObject;

    // Changing crosshair colors when gazing upon an object
    public Image CrosshairImage;
	public Color CrosshairDefaultColor = new Color(1.0f, 1.0f, 1.0f);
	public Color CrosshairGazedColor = new Color(1.0f, 0.0f, 0.0f);

	/// <summary>
	/// Gets a value indicating whether the screen has been touched this frame.
	/// </summary>
	private bool _isScreenTouched
	{
		get
		{
			return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
		}
	}

	/// <summary>
	/// Gets a value indicating whether the VR mode is enabled.
	/// </summary>
	private bool _isVrModeEnabled
	{
		get
		{
			return XRGeneralSettings.Instance.Manager.isInitializationComplete;
		}
	}

	/// <summary>
	/// Start is called before the first frame update.
	/// </summary>
	public void Start()
	{
		// Saves the main camera from the scene.
		_Camera = Camera.main;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f;

		// Checks if the device parameters are stored and scans them if not.
		// This is only required if the XR plugin is initialized on startup,
		// otherwise these API calls can be removed and just be used when the XR
		// plugin is started.
		if (!Api.HasDeviceParams())
		{
			Api.ScanDeviceParams();
		}

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;

		_Controller = GetComponentInParent<CharacterController>();
	}

	public void FixedUpdate()
	{
		// Movement
		if (Camera.main != null)
		{
			float Horizontal = Input.GetAxis("Horizontal");
			float Vertical = Input.GetAxis("Vertical");

			Transform Orientation = Camera.main.transform;
			Vector3 Direction = Orientation.forward * Vertical + Orientation.right * Horizontal;

			Direction.y = 0; // Basically remove noclip movement

			_Controller.Move(Direction * MovementSpeed * Time.deltaTime);
			transform.parent.transform.eulerAngles = new Vector3();
		}
	}

	public void Update()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			MouseX += Input.GetAxis("Mouse X") * 2;
			MouseY -= Input.GetAxis("Mouse Y") * 2;

			transform.eulerAngles = new Vector3(MouseY, MouseX, 0);
		}

		// raycasting to interact with the world objects
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
		{
			// GameObject detected in front of the camera.
			if (hit.transform.gameObject != _CurrentGazedObject)
			{
				// New GameObject.
				if (_CurrentGazedObject != null) SendMessage(_CurrentGazedObject, "OnPointerExit");

				_CurrentGazedObject = hit.transform.gameObject;
				SendMessage(_CurrentGazedObject, "OnPointerEnter");
			}
		}
		else
		{
			// No GameObject detected in front of the camera.
			if (_CurrentGazedObject != null) SendMessage(_CurrentGazedObject, "OnPointerExit");
			_CurrentGazedObject = null;
		}

		if (_CurrentGazedObject != null)
		{
			if (CrosshairImage != null) CrosshairImage.color = CrosshairGazedColor;

			bool InputIsDown = Input.GetMouseButtonDown(0) || Input.GetButtonDown("Attack1") || Input.touchCount > 0;
			if (InputIsDown) SendMessage(_CurrentGazedObject, "OnPointerClick");
		}
		else
		{
			if (CrosshairImage != null) CrosshairImage.color = CrosshairDefaultColor;
		}

		// Foward this line is VR only functions for only android and OpenGLES3
		if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 || Application.platform != RuntimePlatform.Android) return;

		if (_isVrModeEnabled)
		{
			if (Api.IsCloseButtonPressed)
			{
				ExitVR();
			}

			if (Api.IsGearButtonPressed)
			{
				Api.ScanDeviceParams();	
			}

			Api.UpdateScreenParams();
		}
		else
		{
			// TODO(b/171727815): Add a button to switch to VR mode.
			if (_isScreenTouched)
			{
				EnterVR();
			}
		}
	}

	/// <summary>
	/// Enters VR mode.
	/// </summary>
	private void EnterVR()
	{
		StartCoroutine(StartXR());
		if (Api.HasNewDeviceParams())
		{
			Api.ReloadDeviceParams();
		}
	}

	/// <summary>
	/// Exits VR mode.
	/// </summary>
	private void ExitVR()
	{
		StopXR();
	}

	/// <summary>
	/// Initializes and starts the Cardboard XR plugin.
	/// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
	/// </summary>
	///
	/// <returns>
	/// Returns result value of <c>InitializeLoader</c> method from the XR General Settings Manager.
	/// </returns>
	private IEnumerator StartXR()
	{
		Debug.Log("Initializing XR...");
		yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

		if (XRGeneralSettings.Instance.Manager.activeLoader == null)
		{
			Debug.LogError("Initializing XR Failed.");
		}
		else
		{
			Debug.Log("XR initialized.");

			Debug.Log("Starting XR...");
			XRGeneralSettings.Instance.Manager.StartSubsystems();
			Debug.Log("XR started.");
		}
	}

	/// <summary>
	/// Stops and deinitializes the Cardboard XR plugin.
	/// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
	/// </summary>
	private void StopXR()
	{
		Debug.Log("Stopping XR...");
		XRGeneralSettings.Instance.Manager.StopSubsystems();
		Debug.Log("XR stopped.");

		Debug.Log("Deinitializing XR...");
		XRGeneralSettings.Instance.Manager.DeinitializeLoader();
		Debug.Log("XR deinitialized.");

		_Camera.ResetAspect();
		_Camera.fieldOfView = _defaultFieldOfView;
	}

	private void SendMessage(GameObject Object, string Message)
	{
		Object.SendMessage(Message, null, SendMessageOptions.DontRequireReceiver);
	}
}
