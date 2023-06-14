using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.UI;
using TMPro;

public class MainMenuCamera : MonoBehaviour
{
	private const float _defaultFieldOfView = 60.0f;
	Camera _Camera;

	[Header ("Properties")]
	public float MouseSensitivity = 2.0f;
	private float MouseX = 0.0f, MouseY = 0.0f;

	[Header ("Interface")]
	public Button ToggleVRModeButton;

	private bool _isScreenTouched
	{
		get
		{
			return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
		}
	}

	private bool _isVrModeEnabled
	{
		get
		{
			return XRGeneralSettings.Instance.Manager.isInitializationComplete;
		}
	}

	public void Start()
	{
		_Camera = Camera.main;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f;

		ToggleVRModeButton.onClick.AddListener(ToggleVR);
	}

	public void Update()
	{
		/*if (!Application.isMobilePlatform)
		{
			MouseX += Input.GetAxis("Mouse X") * 2;
			MouseY -= Input.GetAxis("Mouse Y") * 2;

			transform.eulerAngles = new Vector3(MouseY, MouseX, 0);
		}*/

		// Desta linha para frente somente para VR / XR no Android
		if (!Application.isMobilePlatform) return;

		if (_isVrModeEnabled)
		{
			if (Api.IsCloseButtonPressed) StopXR();

			/*if (Api.IsGearButtonPressed)
			{
				Api.ScanDeviceParams();	
			}*/

			Api.UpdateScreenParams();
		}
		else
		{
			// TODO(b/171727815): Add a button to switch to VR mode.
			if (_isScreenTouched) EnterVR();
		}
	}

	void ToggleVR()
	{
		if (_isVrModeEnabled) StopXR();
		else EnterVR();
	}

	private void EnterVR()
	{
		Debug.Log("Entering VR mode...");

		StartCoroutine(StartXR());
		if (Api.HasNewDeviceParams())
		{
			Api.ReloadDeviceParams();
		}
	}

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

	private void StopXR()
	{
		Debug.Log("Exiting VR mode");

		Debug.Log("Stopping XR...");
		XRGeneralSettings.Instance.Manager.StopSubsystems();
		Debug.Log("XR stopped.");

		Debug.Log("Deinitializing XR...");
		XRGeneralSettings.Instance.Manager.DeinitializeLoader();
		Debug.Log("XR deinitialized.");

		_Camera.ResetAspect();
		_Camera.fieldOfView = _defaultFieldOfView;
	}
}
