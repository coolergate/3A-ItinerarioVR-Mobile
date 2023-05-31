using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuPortal : MonoBehaviour
{
	private bool _beingGazed = false;
	Renderer _render;

	[Header ("Scenes")]
	public int SceneIndex;
	public LoadSceneMode LoadMode;

	[Header ("Portal properties")]
	public string Portal_Name = "";
	public string Portal_Description = "";
	public Camera Target_Camera;

	[Header ("Text properties")]
	public Canvas Canvas;
	public TextMeshProUGUI TextMesh;

	[Header ("Scene preview properties")]
	public GameObject PreviewSceneObject;

	// Start is called before the first frame update
	void Start()
	{
		_render = GetComponent<MeshRenderer>();

		TextMesh.text = Portal_Name;

		// make everything 
	}

	void Update()
	{
		Vector3 _parent_pos = Canvas.transform.parent.position;

		if (_beingGazed)
		{
			Canvas.transform.position = Vector3.Lerp(Canvas.transform.position, _parent_pos + new Vector3(0, 3f, 0), 0.25f);
		}
		else
		{
			Canvas.transform.position = _parent_pos + new Vector3(0, 2f, 0);
		}

		Canvas.enabled = _beingGazed;
	}

	public void OnActivated(MainMenuPlayer self)
	{
		SceneManager.LoadScene(SceneIndex, LoadMode);
	}

	public void OnPointerEnter()
    {
		_beingGazed = true;
    }

	public void OnPointerClick()
    {
    }

	public void OnPointerExit()
    {
		_beingGazed = false;
    }
}
