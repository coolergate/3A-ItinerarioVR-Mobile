using Google.XR.Cardboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class MainMenu : MonoBehaviour
{
    private const float _defaultFieldOfView = 60.0f;
    private Camera _Camera;

    public void Start()
    {
        _Camera = Camera.main;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;
    }

    public void Update()
    {

        if (!Application.isMobilePlatform) return;

        if (UserSettings.VR_Enabled)
        {
            if (Api.IsCloseButtonPressed) StopXR();

            Api.UpdateScreenParams();
        }
        else
        {
            if (_isScreenTouched) EnterVR();
        }
    }

    void ToggleVR()
    {
        if (UserSettings.VR_Enabled) StopXR();
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
