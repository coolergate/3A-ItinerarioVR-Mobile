using Google.XR.Cardboard;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardboardStartup : MonoBehaviour
{
    public bool LoadPlayerScene = true;
    public bool PlayerMovementEnabled = true;
    public Vector3 PlayerStartingPosition = Vector3.zero;

    [Space(20)]
    public GameObject SoundsParent;

    [Space(20)]
    public string CurrentSceneCaption = "";
    public string NextSceneName = "";

    private float time_left = 0f;

    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f; // only works on iOS

        // Adicionar o jogador de outra cena.
        UserSettings.PlayerStartingPosition = PlayerStartingPosition;
        PlayerController.MovementEnabled = PlayerMovementEnabled;
        if (LoadPlayerScene) SceneManager.LoadScene("Player", LoadSceneMode.Additive);

        void InitSounds()
        {
            StartCoroutine(PlaySounds());
        }
        UserSettings.TransitionEnded.AddListener(InitSounds);

        UserSettings.next_scene_title = CurrentSceneCaption;
    }

    public IEnumerator PlaySounds()
    {
        var listener = gameObject.GetComponent<AudioListener>();
        if (listener == null) listener = gameObject.AddComponent<AudioListener>();

        // get sound length
        float final_time = 0f;
        foreach (var Sound in SoundsParent.GetComponentsInChildren<AudioSource>())
        {
            final_time += Sound.clip.length;
        }
        time_left = final_time;

        foreach (var Sound in SoundsParent.GetComponentsInChildren<AudioSource>())
        {
            Sound.Play();
            yield return new WaitForSeconds(Sound.clip.length);
        }

        yield return new WaitForSeconds(3f);

        UserSettings.FadeoutScreen.Invoke();

        if (NextSceneName != "")
        {
            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene(NextSceneName);
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        UserSettings.TimerText = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Update()
    {
        time_left -= Time.deltaTime;
        if (time_left > 0)
        {
            UpdateTimer(time_left);
        }
        else
        {
            UserSettings.TimerText = "";
        }

        if (Application.isMobilePlatform && UserSettings.VR_Enabled)
        {
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
        };
    }
}
