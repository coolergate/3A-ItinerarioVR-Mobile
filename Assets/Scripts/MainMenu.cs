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

	[Header("Properties")]
	public string FirstSceneName = "";

    [Space(10)]
    public GameObject AnimationObject;
	public Vector3 AnimatedObjectPosition;

	[Space(10)]

	[Header("Interface")]
	public string MobileVRHintText = "";
	public string NonVRHintText = "";

	[Space(10)]
	public GameObject IntroCanvas1;
    public GameObject IntroCanvas2;
    public Image TransitionImage;
	public TextMeshProUGUI HintText;

	private bool _InputActive
	{
		get
		{
			return Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space);
		}
	}

	private IEnumerator InitStage()
	{
        while (true)
        {
            if (_InputActive) break;
            yield return new WaitForEndOfFrame();
        }


		foreach (TextMeshProUGUI text in IntroCanvas1.GetComponentsInChildren<TextMeshProUGUI>())
		{
			void UpdateColor(Color c)
			{
				text.color = c;
			}

            LeanTween.value(text.gameObject, UpdateColor, new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.5f);
        }

		LeanTween.move(AnimationObject, AnimatedObjectPosition, 2f).setEase(LeanTweenType.easeInOutSine);

		yield return new WaitForSeconds(3f);


        foreach (TextMeshProUGUI text in IntroCanvas2.GetComponentsInChildren<TextMeshProUGUI>())
        {
            void UpdateColor(Color c)
            {
                text.color = c;
            }

            LeanTween.value(text.gameObject, UpdateColor, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.5f);
        }

        while (true)
        {
            if (_InputActive) break;
            yield return new WaitForEndOfFrame();
        }

        void UpdateTransitionColor(Color color)
        {
            TransitionImage.color = color;
        }

        LeanTween.value(TransitionImage.gameObject, UpdateTransitionColor, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), 0.5f);

        yield return new WaitForSeconds(1);

		UserSettings.next_scene_title = "Biodiversidade e Qualidade de vida";
        SceneManager.LoadScene(FirstSceneName);
    }

	public void Start()
	{
		_Camera = Camera.main;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f;

		StartCoroutine(InitStage());
	}

	public void Update()
	{
        if (Application.isMobilePlatform && Input.GetJoystickNames().Length > 0)
            HintText.text = MobileVRHintText;
        else
            HintText.text = NonVRHintText;

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

			if (UserSettings.RunningFirstTime)
				UserSettings.RunningFirstTime = false;
		}
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
