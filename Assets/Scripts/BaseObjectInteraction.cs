using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjectInteraction : MonoBehaviour
{
	Renderer _render;

	public Material MaterialDefault;
	public Material Material2;
	public Material Material3;

	// Start is called before the first frame update
	void Start()
	{
		_render = GetComponent<MeshRenderer>();

		
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerEnter()
    {
		_render.material = Material2;
    }

	public void OnPointerClick()
    {
		_render.material = Material3;
    }

	public void OnPointerLeave()
    {
		_render.material = MaterialDefault;
    }
}
