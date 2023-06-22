using Google.XR.Cardboard;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CardboardStartup : MonoBehaviour
{
	public bool LoadPlayerScene = true;
	public bool PlayerMovementEnabled = true;
	public Vector3 PlayerStartingPosition = Vector3.zero;

	[Space(20)]
	public WorldAnimation AnimationScript;
	public float ShowAnimationTime;

	[Space(20)]
	public GameObject SoundsParent;

	[Space(20)]
	public string NextSceneName = "";

	public void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.brightness = 1.0f; // only works on iOS

		// Adicionar o jogador de outra cena.
		UserSettings.PlayerStartingPosition = PlayerStartingPosition;
		PlayerController.MovementEnabled = PlayerMovementEnabled;
		if (LoadPlayerScene) SceneManager.LoadScene("Player", LoadSceneMode.Additive);

		if (AnimationScript)
		{
			AnimationScript.ShowWorld(ShowAnimationTime);
		}
	}

	public IEnumerator PlaySounds()
	{
		foreach(var Sound in SoundsParent.GetComponentsInChildren<AudioSource>())
		{
			Sound.Play();
			yield return new WaitForSeconds(Sound.clip.length);
		}

		if (NextSceneName != "")
		{
			yield return new WaitForSeconds(10f);

			SceneManager.LoadScene(NextSceneName);
		}
	}

	public void Update()
	{
		if (!Application.isMobilePlatform) return;

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
