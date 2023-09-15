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

    public static bool MovementEnabled = true;
    public static CardboardStartup CurrentWorldController;

    [Header("Properties")]
    public float CharacterMovementSpeed = 10.0f;
    public float CameraSensitivity = 2.0f;
    private float CameraX = 0.0f, CameraY = 0.0f;

    [Space(20)]
    public Image TransitionImage;
    public TextMeshProUGUI TransitionCaptionText;
    public TextMeshProUGUI TimerText;

    [Space(20)]
    public AudioSource TransitionAudio;
    public FixedJoystick MovementJoystickHandler;
    public FixedJoystick CameraJoystickHandler;

    private ObjectInteractionHandler _currentObjectController;
    private Vector3 _current_velocity = new Vector3();

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

    public IEnumerator Start()
    {
        _Camera = Camera.main;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        _Controller = GetComponentInParent<CharacterController>();

        TransitionCaptionText.color = new Color(0, 0, 0, 0);
        TransitionImage.color = new Color(0, 0, 0, 1);

        TransitionCaptionText.text = UserSettings.next_scene_title;

        UserSettings.CurrentPlayerController = this;
        transform.parent.transform.position = UserSettings.PlayerStartingPosition;

        void StartFadeout()
        {
            StartCoroutine(FadeoutScreen());
        }
        UserSettings.FadeoutScreen.AddListener(StartFadeout);

        yield return new WaitForSeconds(2);

        StartCoroutine(StartTransition());
    }

    public IEnumerator StartTransition()
    {
        if (Application.isMobilePlatform) TransitionAudio.Play();
        Destroy(gameObject.GetComponent<AudioListener>(), TransitionAudio.clip.length);

        void UpdateTransitionColor(Color c)
        {
            TransitionImage.color = c;
        }
        void UpdateCaptionColor(Color c)
        {
            TransitionCaptionText.color = c;
        }

        LeanTween.value(TransitionImage.gameObject, UpdateTransitionColor, new Color(0, 0, 0, 1), new Color(1, 1, 1, 1), 1f);

        yield return new WaitForSeconds(1.825f);

        TransitionCaptionText.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(0.5f);

        LeanTween.value(TransitionImage.gameObject, UpdateTransitionColor, new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 1f);
        LeanTween.value(TransitionCaptionText.gameObject, UpdateCaptionColor, new Color(0, 0, 0, 1), new Color(1, 1, 1, 1), 1f);

        yield return new WaitForSeconds(6f);

        LeanTween.value(TransitionCaptionText.gameObject, UpdateCaptionColor, new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 2f);

        yield return new WaitForSeconds(1f);

        UserSettings.TransitionEnded.Invoke();
    }

    public IEnumerator FadeoutScreen()
    {
        Debug.Log("Starting fadeout");
        void UpdateTransitionColor(Color c)
        {
            TransitionImage.color = c;
        }
        LeanTween.value(TransitionImage.gameObject, UpdateTransitionColor, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), 1f);

        yield return new WaitForSeconds(2f);
    }

    public void FixedUpdate()
    {
        if (_Controller == null || !MovementEnabled) return;

        // movement
        if (Camera.main != null)
        {
            Vector2 MovementInput = MovementJoystickHandler.Direction;

            Transform Orientation = Camera.main.transform;
            Vector3 Direction = Orientation.forward * MovementInput.y + Orientation.right * MovementInput.x;
            Direction.y = -0.25f;

            _current_velocity = Vector3.Lerp(_current_velocity, Direction, 0.125f);
        }

        _Controller.Move(_current_velocity * CharacterMovementSpeed * Time.deltaTime);

        if (!_isVrModeEnabled)
        {
            Vector2 CameraInput = CameraJoystickHandler.Direction * CameraSensitivity;
            CameraX += CameraInput.x;
            CameraY -= CameraInput.y;

            transform.eulerAngles = new Vector3(CameraY, CameraX, 0);
            transform.SetLocalPositionAndRotation(new Vector3(), transform.rotation);
        }
    }

    public void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (!_isVrModeEnabled) EnterVR();
            else Api.UpdateScreenParams();
        }

        TimerText.text = UserSettings.TimerText;

        if (Input.GetButton("Exit")) Application.Quit();
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

    public IEnumerator PlayAudios(GameObject parent)
    {
        foreach (var Sound in parent.GetComponentsInChildren<AudioSource>())
        {
            Sound.Play();
            yield return new WaitForSeconds(Sound.clip.length);
        }
    }
}
