using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.XR.Management;

public class UserSettings : MonoBehaviour
{
    public static bool VR_Enabled
    {
        get
        {
            return XRGeneralSettings.Instance.Manager.isInitializationComplete;
        }
    }

    public static bool isScreenBeingTouched
    {
        get
        {
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }

    public static bool RunningFirstTime = true;
    public static WorldAnimation CurrentWorldAnimationInstance;

    public static string next_scene_title = "UndefinedSceneName";

    public static Vector3 PlayerStartingPosition = new Vector3(0, 1, 0);
    public static PlayerController CurrentPlayerController;
}
