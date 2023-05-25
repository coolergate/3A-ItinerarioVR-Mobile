using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.UI;
using TMPro;

public class MainMenuPlayer : MonoBehaviour
{
	private const float _defaultFieldOfView = 60.0f;
	Camera _Camera;
	CharacterController _Controller;

	[Header ("Properties")]
	public float MouseSensitivity = 2.0f;
	private float MouseX = 0.0f, MouseY = 0.0f;

	[Header ("Interface")]
	public Image CrosshairImage;
	public Color CrosshairDefaultColor = new Color(1.0f, 1.0f, 1.0f);
	public Color CrosshairGazedColor = new Color(1.0f, 1.0f, 0.0f);

	public GameObject HoverUIElement; 

	public TextMeshProUGUI DescriptionText;

	private MainMenuPortal _currentPortal;

	private bool _isScreenTouched
	{
		get
		{
			return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
		}
	}
	private bool _isInputActive
	{
		get
		{
			bool physical_input = Input.GetMouseButtonDown(0) || Input.GetButtonDown("Attack1");
			bool touch_input = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
			return physical_input || touch_input;
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

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;

		_Controller = GetComponentInParent<CharacterController>();
	}

	public void Update()
	{
		if (!Application.isMobilePlatform)
		{
			MouseX += Input.GetAxis("Mouse X") * 2;
			MouseY -= Input.GetAxis("Mouse Y") * 2;

			transform.eulerAngles = new Vector3(MouseY, MouseX, 0);
		}

		// Raycast para interagir com os objetos da cena
		MainMenuPortal GazedPortal = RaycastInteractableObject();

		if (GazedPortal != _currentPortal)
		{
			if (_currentPortal != null) _currentPortal.OnPointerExit();
				//SendMessage(_currentObject, "OnPointerExit");
				

			if (GazedPortal) GazedPortal.OnPointerEnter();
				//SendMessage(_currentObject, "OnPointerEnter");

			_currentPortal = GazedPortal;
		}

		if (_currentPortal != null)
		{
			if (CrosshairImage != null) CrosshairImage.color = CrosshairGazedColor;
			if (HoverUIElement) HoverUIElement.SetActive(true);

			if (DescriptionText)
			{
				DescriptionText.gameObject.SetActive(true);
				DescriptionText.text = GazedPortal.PortalName;
			}

			if (_isInputActive) _currentPortal.OnActivated(this);
				//SendMessage(_currentObject, "OnPointerClick");
		}
		else
		{
			if (CrosshairImage != null) CrosshairImage.color = CrosshairDefaultColor;
			if (HoverUIElement) HoverUIElement.SetActive(false);
			if (DescriptionText) DescriptionText.gameObject.SetActive(false);
			
		}

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

	private void EnterVR()
	{
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

	MainMenuPortal RaycastInteractableObject()
	{
		RaycastHit hit;
		bool success_raycast = Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);

		if (success_raycast)
		{
			GameObject GazedObject = hit.transform.gameObject;
			MainMenuPortal GazedObjectController = GazedObject.GetComponent<MainMenuPortal>();

			if (GazedObjectController != null) return GazedObjectController;
		}

		return null;
	}
}
