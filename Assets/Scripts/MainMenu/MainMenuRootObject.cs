using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuRootObject : MonoBehaviour
{
	[Header ("Properties")]
	public float RotationAmount = 0.0f;
	
	[Header ("Interface properties")]
	public TextMeshProUGUI DescriptionText; 

	private int _index = 0;
	private GameObject[] _children_list = {};

	private bool _allowedupdate = true;

	// Start is called before the first frame update
	void Start()
	{
		for (int index = 0; index < transform.childCount; index++)
		{
			GameObject child = transform.GetChild(index).gameObject;
			
			if (!child.GetComponent<MainMenuPortal>()) continue;

			_children_list.SetValue(child, _children_list.Length + 1);
		}
	}

	// Update is called once per frame
	void Update()
	{
		int move_direction = 0;

		if (_allowedupdate)
		{
			float direction = Input.GetAxis("Horizontal");
			move_direction = direction > 0 ? 1 : -1;
		}

		if (move_direction != 0) RotateObject(move_direction);
	}

	private IEnumerator RotateObject(int direction)
	{
		_allowedupdate = false;
		LeanTween.rotateAround(gameObject, Vector3.up, RotationAmount * direction, 0.5f);

		yield return new WaitForSeconds(0.5f);

		_allowedupdate = true;
	}

	void Select()
	{

	}
}
