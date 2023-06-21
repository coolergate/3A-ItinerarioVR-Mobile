using System.Collections.Generic;
using UnityEngine;

public class WorldAnimation : MonoBehaviour
{
    Dictionary<GameObject, Vector3> _PositionList = new Dictionary<GameObject, Vector3>();
    Dictionary<GameObject, Color> _ColorList = new Dictionary<GameObject, Color>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var render in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            _PositionList.Add(render.gameObject, render.gameObject.transform.position);
            _ColorList.Add(render.gameObject, render.material.color);

            var pos = render.gameObject.transform.position;
            pos.y -= 200;

            var col = render.material.color;
            col.a = 0;

            render.gameObject.transform.position = pos;
            render.material.color = col;
        }

        UserSettings.CurrentWorldAnimationInstance = this;
    }

    public void ShowWorld()
    {
        foreach (var render in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            var original_color = _ColorList[render.gameObject];
            var original_position = _PositionList[render.gameObject];
            var random_interval = 2 * (Random.Range(0, 500) * 0.1f);

            void UpdateColor(Color c)
            {
                render.material.color = c;
            }

            LeanTween.value(gameObject, UpdateColor, render.material.color, original_color, random_interval);
            LeanTween.move(gameObject, original_position, random_interval).setEase(LeanTweenType.easeOutSine);
        }
    }

    public void HideWorld()
    {
        foreach (var render in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            var original_color = _ColorList[render.gameObject];
            var random_interval = 2 * (Random.Range(0, 500) * 0.1f);

            var hidden_color = _ColorList[render.gameObject];
            hidden_color.a = 0;

            var hidden_position = _PositionList[render.gameObject] - new Vector3(0, 200, 0);

            void UpdateColor(Color c)
            {
                render.material.color = c;
            }

            LeanTween.value(gameObject, UpdateColor, original_color, hidden_color, random_interval);
            LeanTween.move(gameObject, hidden_position, random_interval).setEase(LeanTweenType.easeInSine);
        }
    }
}
