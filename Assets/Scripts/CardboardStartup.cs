using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Inicia o plugin Cardboard XR.
/// </summary>
public class CardboardStartup : MonoBehaviour
{
	public void Start()
	{
		// Configura o aplicativo para nao desligar a tela e aumenta o brilho no maximo.
		// Controle de brilho e esperado para somente funcionar no iOS, olhe:
		// https://docs.unity3d.com/ScriptReference/Screen-brightness.html.
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f;

		// Verifica se o dispositivo tem os par�metros guardados e escaneia caso n�o.
		if (!Api.HasDeviceParams())
		{
			Api.ScanDeviceParams();
		}

		// Adicionar o jogador de outra cena.
		SceneManager.LoadScene(1, LoadSceneMode.Additive);
	}

	public void Update()
	{

		//if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3) return;
		if (Application.platform != RuntimePlatform.Android) return;
		
		/*if (Api.IsGearButtonPressed)
		{
			Api.ScanDeviceParams();
		}*/

		if (Api.IsCloseButtonPressed)
		{
			Application.Quit();
		}

		if (Api.IsTriggerHeldPressed)
		{
			Api.Recenter();
		}

		if (Api.HasNewDeviceParams())
		{
			Api.ReloadDeviceParams();
		}

		Api.UpdateScreenParams();
	}
}
