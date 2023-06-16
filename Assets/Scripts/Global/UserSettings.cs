using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class UserSettings : MonoBehaviour
{
	public static List<string> CompletedScenes = new List<string>();

    public static bool VR_Enabled
    {
        get
        {
            return XRGeneralSettings.Instance.Manager.isInitializationComplete;
        }
    }

    public static bool isScreenTouched
    {
        get
        {
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }
}
