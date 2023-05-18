using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Button PlayButton;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        PlayButton.onClick.AddListener(SetScene);
    }

    public void SetScene()
    {
        SceneManager.UnloadSceneAsync(0);
        SceneManager.UnloadSceneAsync(2); // Reload player
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
