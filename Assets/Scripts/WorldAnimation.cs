using System.Collections.Generic;
using UnityEngine;

public class WorldAnimation : MonoBehaviour
{
    Dictionary<GameObject, Vector3> _PositionList = new Dictionary<GameObject, Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var render in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            _PositionList.Add(render.gameObject, render.gameObject.transform.position);
            render.gameObject.transform.position -= new Vector3(0, 200, 0);
        }

        UserSettings.CurrentWorldAnimationInstance = this;
    }

    public void ShowWorld(float TimeToFinish)
    {
        foreach (var render in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            var original_position = _PositionList[render.gameObject];
            var random_interval = TimeToFinish * (Random.Range(0, 500) * 0.01f);

            LeanTween.move(gameObject, original_position, random_interval).setEase(LeanTweenType.easeOutSine);
        }
    }

    public void HideWorld(float TimeToFinish)
    {
        foreach (var render in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            var hidden_position = _PositionList[render.gameObject] - new Vector3(0, 200, 0);
            var random_interval = TimeToFinish * (Random.Range(0, 500) * 0.01f);
            LeanTween.move(gameObject, hidden_position, random_interval).setEase(LeanTweenType.easeInSine);
        }
    }
}
