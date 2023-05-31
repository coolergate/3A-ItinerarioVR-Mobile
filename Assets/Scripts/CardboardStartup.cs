using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CardboardStartup : MonoBehaviour
{
	public void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f; // only works on iOS

		// Adicionar o jogador de outra cena.
		SceneManager.LoadScene(2, LoadSceneMode.Additive);
	}

	public void Update()
	{

		//if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3) return;
		if (!Application.isMobilePlatform) return;

		if (Api.IsCloseButtonPressed)
		{
			Application.Quit();
		}

		/*if (Api.IsTriggerHeldPressed)
		{
			Api.Recenter();
		}*/

		/*if (Api.HasNewDeviceParams())
		{
			Api.ReloadDeviceParams();
		}*/

		Api.UpdateScreenParams();
	}
}
