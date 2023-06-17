using Google.XR.Cardboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	private const float _defaultFieldOfView = 60.0f;
	private Camera _Camera;
	private bool _allowed_update = true;

	[Header("Properties")]
	public string FirstSceneName = "";

	[Space(10)]

	[Header("Interface")]
	public Image TransitionImage;

	[Space(10)]
	public TextMeshProUGUI MobileDescriptionText;

	[Space(10)]
	public Button PlayButton;
	public Button SettingsButton;

	public void Start()
	{
		_Camera = Camera.main;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f;

		PlayButton.onClick.AddListener(EnterScene);
		MobileDescriptionText.gameObject.SetActive(Application.isMobilePlatform && UserSettings.VR_Enabled);
	}

	public void Update()
	{
		if (!_allowed_update) return;

		if (Application.isMobilePlatform)
		{
			if (UserSettings.VR_Enabled)
			{
				if (Api.IsCloseButtonPressed) StopXR();

				Api.UpdateScreenParams();
			}
			else
			{
				if (UserSettings.isScreenBeingTouched) EnterVR();
			}

			if (UserSettings.RunningFirstTime && UserSettings.VR_Enabled)
			{
				UserSettings.RunningFirstTime = false;
				StopXR();
			}
		}


		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button4))
		{
			_allowed_update = false;

			EnterScene();
		}
    }

	private void EnterScene()
	{
		StartCoroutine(StartStory());
	}

	private IEnumerator StartStory()
	{
        var current_color = TransitionImage.color;
        current_color.a = 0;

        var target_color = TransitionImage.color;
        target_color.a = 1;

		void UpdateTransitionColor(Color color)
		{
			TransitionImage.color = color;	
		}

		LeanTween.value(TransitionImage.gameObject, UpdateTransitionColor, current_color, target_color, 0.5f);

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(FirstSceneName);
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
