using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuRootObject : MonoBehaviour
{
	[Header("Properties")]
	public float RotationAmount = 0.0f;
	public float RotationTime = 0.25f;

	[Header("Interface properties")]
	public TextMeshProUGUI DescriptionText;

	private int _index = 0;
	private List<MainMenuPortal> _children_list = new List<MainMenuPortal>();

	private MainMenuPortal _current_portal_component;
	private bool _allowedupdate = true;
	private bool _inputheld = false;

	// Start is called before the first frame update
	void Start()
	{
		foreach (MainMenuPortal component in gameObject.GetComponentsInChildren<MainMenuPortal>())
		{
			_children_list.Add(component);

			LeanTween.alpha(component.PreviewObject, 0.0f, RotationTime * 0.5f);

			component.PreviewObject.SetActive(false);
		}

		StartCoroutine(ChangeOption(1));
	}

	// Update is called once per frame
	void Update()
	{
		if (!_allowedupdate) return;

		float direction = Input.GetAxis("Horizontal");

		if (direction != 0.0f)
		{
			if (_inputheld == false) StartCoroutine(ChangeOption(direction > 0 ? -1 : 1));
			_inputheld = true;
		}
		else _inputheld = false;
	}

	private IEnumerator ChangeOption(int direction)
	{
		_allowedupdate = false;
		LeanTween
			.rotateAround(gameObject, Vector3.up, RotationAmount * direction, RotationTime)
			.setEase(LeanTweenType.easeOutQuad);

		_index += direction;
		if (_index == _children_list.Count) _index = 0;
		if (_index < 0) _index = _children_list.Count - 1;

		MainMenuPortal SelectedObject = _children_list[_index];

		LeanTween.textAlpha(DescriptionText.rectTransform, 0.0f, RotationTime * 0.5f);

		if (_current_portal_component)
		{
			LeanTween.alpha(_current_portal_component.PreviewObject, 0.0f, RotationTime * 0.5f);

			yield return new WaitForSeconds(RotationTime * 0.5f);

			_current_portal_component.PreviewObject.SetActive(false);
		}

		if (SelectedObject != null)
		{
			SelectedObject.PreviewObject.SetActive(true);
			LeanTween.alpha(SelectedObject.PreviewObject, 0.5f, RotationTime * 0.5f);

			DescriptionText.text = SelectedObject.PortalName;
		}
		else DescriptionText.text = "";

		LeanTween.textAlpha(DescriptionText.rectTransform, 1.0f, RotationTime * 0.5f);

		yield return new WaitForSeconds(RotationTime * 0.5f);

		_current_portal_component = SelectedObject;
		_allowedupdate = true;
	}

	void Select()
	{

	}
}
