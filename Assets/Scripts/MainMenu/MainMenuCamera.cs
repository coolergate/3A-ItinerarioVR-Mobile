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

	public MainMenuRootObject RootObject;

	[Header ("Interface")]
	public GameObject NonVRInterfaceHolder;

	public Button EnableVR;
    public Button Select;
	public Button Previous;
    public Button Next;

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

		EnableVR.onClick.AddListener(ToggleVR);
		EnableVR.gameObject.SetActive(Application.isMobilePlatform);

		Select.onClick.AddListener(SubmitSelection);
		Next.onClick.AddListener(SelectRight);
        Previous.onClick.AddListener(SelectLeft);
    }

	public void SelectLeft()
	{
		RootObject.StartCoroutine(RootObject.ChangeOption(1));
	}
    public void SelectRight()
    {
        RootObject.StartCoroutine(RootObject.ChangeOption(-1));
    }

    public void SubmitSelection()
	{
		RootObject.StartCoroutine(RootObject.LoadSelectedScene());
	}

	public void Update()
	{
		
		EnableVR.gameObject.SetActive(Application.isMobilePlatform && _isVrModeEnabled);

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
