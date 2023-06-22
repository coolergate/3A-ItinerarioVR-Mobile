using Google.XR.Cardboard;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Management;

public class PlayerController : MonoBehaviour
{
    private const float _defaultFieldOfView = 60.0f;
    Camera _Camera;
    CharacterController _Controller;

    [Header("Properties")]
    public float CharacterMovementSpeed = 10.0f;
    public float MouseSensitivity = 2.0f;
    private float MouseX = 0.0f, MouseY = 0.0f;

    [Header("Interface")]
    public Image CrosshairImage;
    public Color CrosshairDefaultColor = new Color(1.0f, 1.0f, 1.0f);
    public Color CrosshairGazedColor = new Color(1.0f, 0.0f, 0.0f);

    [Space(20)]
    public Image TransitionImage;
    public TextMeshProUGUI CaptionText;

    [Header("Properties")]
    public GameObject AudiosObjects;

    private ObjectInteractionHandler _currentObjectController;
    private Vector3 _current_velocity = new Vector3();

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

        void UpdateTransitionColor(Color c)
        {
            TransitionImage.color = c;
        }

        var current_color = new Color(1, 1, 1, 1);
        var target_color = new Color(1, 1, 1, 0);

        LeanTween.value(TransitionImage.gameObject, UpdateTransitionColor, current_color, target_color, 1.5f);

        GetComponentInParent<Transform>().position = UserSettings.PlayerStartingPosition;
    }

    public void FixedUpdate()
    {
        if (_Controller == null) return;

        if (Camera.main != null)
        {
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");

            Transform Orientation = Camera.main.transform;
            Vector3 Direction = Orientation.forward * Vertical + Orientation.right * Horizontal;
            Direction.y = 0;

            _current_velocity = Vector3.Lerp(_current_velocity, Direction, 0.125f);
        }

        _Controller.Move(_current_velocity * CharacterMovementSpeed * Time.deltaTime);
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
        ObjectInteractionHandler GazedObjectController = RaycastInteractableObject();

        if (GazedObjectController != _currentObjectController)
        {
            if (_currentObjectController != null) _currentObjectController.OnPointerExit();
            //SendMessage(_currentObject, "OnPointerExit");


            if (GazedObjectController) GazedObjectController.OnPointerEnter();
            //SendMessage(_currentObject, "OnPointerEnter");

            _currentObjectController = GazedObjectController;
        }

        if (_currentObjectController != null)
        {
            if (CrosshairImage != null) CrosshairImage.color = CrosshairGazedColor;

            if (_isInputActive) _currentObjectController.OnPointerClick();
            //SendMessage(_currentObject, "OnPointerClick");
        }
        else
        {
            if (CrosshairImage != null) CrosshairImage.color = CrosshairDefaultColor;
        }

        if (Application.isMobilePlatform)
        {
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

    ObjectInteractionHandler RaycastInteractableObject()
    {
        RaycastHit hit;
        bool success_raycast = Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);

        if (success_raycast)
        {
            GameObject GazedObject = hit.transform.gameObject;
            ObjectInteractionHandler GazedObjectController = GazedObject.GetComponent<ObjectInteractionHandler>();

            if (GazedObjectController != null) return GazedObjectController;
        }

        return null;
    }

    private IEnumerator LoadAudioSources()
    {
        yield return new WaitForSeconds(3);

        foreach (var companion in AudiosObjects.GetComponentsInChildren<AudioCompanion>())
        {
            if (companion.clip != null)
            {
                companion.audioSource.clip = companion.clip;
                companion.audioSource.Play();
                yield return new WaitForSeconds(companion.clip.length);
            }

            continue;
        }
    }
}
