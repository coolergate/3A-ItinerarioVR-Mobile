using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuPortal : MonoBehaviour
{
	Renderer _render;

	[Header ("Materials")]
	public Material MaterialDefault;
	public Material Material1;
	public Material Material2;

	[Header ("Scenes")]
	public int SceneIndex;
	public LoadSceneMode LoadMode;

	[Header ("Portal properties")]
	public string PortalName = "";
	public string PortalDescription = "";

	[Header ("Text properties")]
	public Canvas Canvas;
	public TextMeshProUGUI TextMesh;

	private bool IsBeingGazed = false;

	// Start is called before the first frame update
	void Start()
	{
		_render = GetComponent<MeshRenderer>();

		_render.material = MaterialDefault;
	}

	// Update is called once per frame
	void Update()
	{
		if (IsBeingGazed)
		{
			Canvas.transform.position = Vector3.Lerp(Canvas.transform.position, new Vector3());
		}
	}

	public void OnActivated(MainMenuPlayer self)
	{
		SceneManager.LoadScene(SceneIndex, LoadMode);


	}

	public void OnPointerEnter()
    {
		_render.material = Material1;
    }

	public void OnPointerClick()
    {
		_render.material = Material2;
    }

	public void OnPointerExit()
    {
		_render.material = MaterialDefault;
    }
}
