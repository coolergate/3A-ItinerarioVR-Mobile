using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuRootObject : MonoBehaviour
{
	[Header ("Properties")]
	public float RotationAmount = 0.0f;
	public float RotationTime = 0.25f;
	
	[Header ("Interface properties")]
	public TextMeshProUGUI DescriptionText; 

	private int _index = 0;
	private int _portals_amount = 0;
	private ArrayList _children_list;

	private MainMenuPortal _current_portal_component;
	private bool _allowedupdate = true;

	// Start is called before the first frame update
	void Start()
	{
		_children_list = new ArrayList(transform.childCount);

		for (int index = 0; index < transform.childCount; index++)
		{
			GameObject child = transform.GetChild(index).gameObject;
			
			MainMenuPortal component;
			child.TryGetComponent<MainMenuPortal>(out component);
			if (!component) continue;

			Color ComponentColor = component.PreviewRenderer.material.color;
			ComponentColor.a = 0.0f;

			//component.PreviewRenderer.material.color = ComponentColor;
			component.PreviewRenderer.material.color = new Color(0, 0, 0, 0);

			_portals_amount++;
			_children_list.Add(child);
		}

		Debug.Log(_children_list.Count);
	}

	// Update is called once per frame
	void Update()
	{
		if (!_allowedupdate) return;

		float direction = Input.GetAxis("Horizontal");
		if (direction != 0.0f) StartCoroutine(ChangeOption(direction > 0 ? -1 : 1));
	}

	private IEnumerator ChangeOption(int direction)
	{
		_allowedupdate = false;

		LeanTween.rotateAround(gameObject, Vector3.up, RotationAmount * direction, RotationTime).setEase(LeanTweenType.easeOutQuad);

		if (_current_portal_component)
		{
			Color component_color = _current_portal_component.PreviewRenderer.material.color;
			component_color.a = 0.0f;
			LeanTween.color(_current_portal_component.gameObject, new Color(1, 1, 1, 0), RotationTime * 0.5f);
		}

		yield return new WaitForSeconds(RotationTime * 0.5f);

		_index += direction;
		if (_index > _children_list.Count - 1 || _index < 0) _index = 0;

		_current_portal_component = _children_list[_index] as MainMenuPortal;

		if (_current_portal_component)
			LeanTween.color(_current_portal_component.gameObject, new Color(1, 1, 1, 1), RotationTime * 0.5f);

		yield return new WaitForSeconds(RotationTime * 0.5f	);

		_allowedupdate = true;
	}

	void Select()
	{

	}
}
