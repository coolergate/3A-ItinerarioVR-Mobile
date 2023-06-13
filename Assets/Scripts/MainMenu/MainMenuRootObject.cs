using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuRootObject : MonoBehaviour
{
	[Header("Properties")]
	public float RotationAmount = 0.0f;
	public float RotationTime = 0.25f;

	[Header("Interface properties")]
	public TextMeshProUGUI PortalNameText;
	public TextMeshProUGUI PortalDescriptionText;

	public string DefaultName = "";
	public string DefaultDescription = "";

	private int _index = 0;
	private List<MainMenuPortal> _children_list = new List<MainMenuPortal>();

	private MainMenuPortal _current_portal_component;
	private bool _allowedupdate = true;
	private bool _inputheld = false;

	private bool _isInputActive
	{
		get
		{
			bool physical_input = Input.GetMouseButtonDown(0) || Input.GetButtonDown("Attack1");
			bool touch_input = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
			return physical_input || touch_input;
		}
	}

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

		if (_isInputActive && _current_portal_component)
		{
			var scene_index = _current_portal_component.SceneIndex;

			SceneManager.LoadScene(scene_index, LoadSceneMode.Single);
		}
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

		LeanTween.textAlpha(PortalNameText.rectTransform, 0.0f, RotationTime * 0.5f);

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

			PortalNameText.text = SelectedObject.PortalName;
			PortalDescriptionText.text = SelectedObject.PortalDescription;
		}
		else { PortalNameText.text = DefaultName; PortalDescriptionText.text = DefaultDescription; }

		LeanTween.textAlpha(PortalNameText.rectTransform, 1.0f, RotationTime * 0.5f);

		yield return new WaitForSeconds(RotationTime * 0.5f);

		_current_portal_component = SelectedObject;
		_allowedupdate = true;
	}

	void Select()
	{

	}
}
