using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionHandler : MonoBehaviour
{
	Renderer _render;

	public Material MaterialDefault;
	public Material Material1;
	public Material Material2;

	// Start is called before the first frame update
	void Start()
	{
		_render = GetComponent<MeshRenderer>();

		_render.material = MaterialDefault;
	}

	// Update is called once per frame
	void Update()
	{
		
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
