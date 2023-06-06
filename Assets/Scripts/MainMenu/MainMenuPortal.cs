using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPortal : MonoBehaviour
{
	[Header ("Scenes")]
	public int SceneIndex;
	public LoadSceneMode LoadMode;

	[Header ("Portal properties")]
	public string PortalName = "";
	public string PortalDescription = "";
	public GameObject PreviewObject;

	void Start()
	{

	}

	void Update()
	{

	}
}
