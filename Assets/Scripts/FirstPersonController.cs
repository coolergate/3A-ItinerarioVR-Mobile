using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.UI;

/// <summary>
/// Modulo que controla a perspectiva do jogador em primeira pessoa
/// </summary>
public class FirstPersonController : MonoBehaviour
{
	// Campo de vis�o para quando o modo VR n�o estiver ativo
	private const float _defaultFieldOfView = 60.0f;

	private Camera _Camera;

	// Movimento do mouse para computador
	public float MouseSensitivity = 2.0f;
	private float MouseX = 0.0f, MouseY = 0.0f;

	// Propriedades de movimento do jogador
	CharacterController _Controller;
	public float MovementSpeed = 10.0f;

	// Ponteiro para indicar objeto que est� sendo apontado
	private GameObject _CurrentGazedObject;

    // Propriedades do crosshair para alterar quando tiver um objeto ativo
    public Image CrosshairImage;
	public Color CrosshairDefaultColor = new Color(1.0f, 1.0f, 1.0f);
	public Color CrosshairGazedColor = new Color(1.0f, 0.0f, 0.0f);

	/// <summary>
	/// Retorna se a tela est� sendo tocada neste frame
	/// </summary>
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

	/// <summary>
	/// Retorna uma vari�vel indicando se o modo VR est� ativo
	/// </summary>
	private bool _isVrModeEnabled
	{
		get
		{
			return XRGeneralSettings.Instance.Manager.isInitializationComplete;
		}
	}

	public void Start()
	{
		// Salva a c�mera principal da cena
		_Camera = Camera.main;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f;

		// Verifica se os par�metros do dispositivo est� guardado, scanea se n�o.
		// Isso � somente obrigat�rio se o plugin XR est� ativo na inicializa��o,
		// se n�o esses chamados da API podem ser removidos e somente ser usado
		// quando o plugin XR estiver ativo.
		if (!Api.HasDeviceParams())
		{
			Api.ScanDeviceParams();
		}

		// Remover o cursor da tela e trancar para que n�o saia do centro
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;

		_Controller = GetComponentInParent<CharacterController>();
	}

	public void FixedUpdate()
	{
		// Movimentacao
		if (Camera.main != null)
		{
			float Horizontal = Input.GetAxis("Horizontal");
			float Vertical = Input.GetAxis("Vertical");

			Transform Orientation = Camera.main.transform;
			Vector3 Direction = Orientation.forward * Vertical + Orientation.right * Horizontal;

			// Remover movimento vertical para que o jogador n�o fique voando pela cena
			Direction.y = 0;

			_Controller.Move(Direction * MovementSpeed * Time.deltaTime);
			transform.parent.transform.eulerAngles = new Vector3();
		}
	}

	public void Update()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			MouseX += Input.GetAxis("Mouse X") * 2;
			MouseY -= Input.GetAxis("Mouse Y") * 2;

			transform.eulerAngles = new Vector3(MouseY, MouseX, 0);
		}

		// Raycast para interagir com os objetos da cena
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
		{

			if (hit.transform.gameObject != _CurrentGazedObject)
			{

				if (_CurrentGazedObject != null) SendMessage(_CurrentGazedObject, "OnPointerExit");

				_CurrentGazedObject = hit.transform.gameObject;
				SendMessage(_CurrentGazedObject, "OnPointerEnter");

			}

		}
		else
		{

			// Nenhum objeto detectado em frente a camera
			if (_CurrentGazedObject != null) SendMessage(_CurrentGazedObject, "OnPointerExit");
			_CurrentGazedObject = null;

		}

		if (_CurrentGazedObject != null && _CurrentGazedObject.tag == "VR_Interactable")
		{

			if (CrosshairImage != null) CrosshairImage.color = CrosshairGazedColor;

			if (_isInputActive) SendMessage(_CurrentGazedObject, "OnPointerClick");
			
		}
		else
		{

			if (CrosshairImage != null) CrosshairImage.color = CrosshairDefaultColor;

		}

		// Desta linha para frente somente para VR / XR no Android
		if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 || Application.platform != RuntimePlatform.Android) return;

		if (_isVrModeEnabled)
		{
			if (Api.IsCloseButtonPressed)
			{
				ExitVR();
			}

			if (Api.IsGearButtonPressed)
			{
				Api.ScanDeviceParams();	
			}

			Api.UpdateScreenParams();
		}
		else
		{
			// TODO(b/171727815): Add a button to switch to VR mode.
			if (_isScreenTouched)
			{
				EnterVR();
			}
		}
	}

	/// <summary>
	/// Entrar no modo VR.
	/// </summary>
	private void EnterVR()
	{
		StartCoroutine(StartXR());
		if (Api.HasNewDeviceParams())
		{
			Api.ReloadDeviceParams();
		}
	}

	/// <summary>
	/// Sair do modo VR.
	/// </summary>
	private void ExitVR()
	{
		StopXR();
	}

	/// <summary>
	/// Inicializa o plugin Cardboard XR
	/// Olhe https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
	/// </summary>
	///
	/// <returns>
	/// Retorna a vari�vel do m�todo <c>InitializeLoader</c> do arquivo de configa��o XR Geral.
	/// </returns>
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

	/// <summary>
	/// Stops and deinitializes the Cardboard XR plugin.
	/// Encerra o plugin Cardboard XR
	/// Olhe https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
	/// </summary>
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
}
