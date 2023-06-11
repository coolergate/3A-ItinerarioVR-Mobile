using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPortal : MonoBehaviour
{
	[Header ("Portal properties")]
	public string PortalName = "";
	public string PortalDescription = "";
	public int SceneIndex;

	[Header ("Preview object properties")]
	public GameObject PreviewObject;

	private Color _original_color;

	public Color original_color
	{
		get {
			return _original_color;
		}
	}

	void Start()
	{
	}

	void Update()
	{

	}
}
