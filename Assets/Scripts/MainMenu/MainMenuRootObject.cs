using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuRootObject : MonoBehaviour
{
	[Header("Properties")]
	public float RotationAmount = 0.0f;
	public float RotationTime = 0.25f;

	[Header("Interface properties")]
	public TextMeshProUGUI PortalNameText;
	public TextMeshProUGUI PortalDescriptionText;

	[Space(20)]
	public string DefaultName = "";
	public string DefaultDescription = "";

	[Space(20)]
	public Image TransitionImage;

	private int _index = 0;
	private List<MainMenuPortal> _children_list = new List<MainMenuPortal>();

	private MainMenuPortal _current_portal_component;
	private bool _allowedupdate = true;
	private bool _inputheld = false;

	private bool _isInputActive
	{
		get
		{
			bool physical_input = Input.GetButtonDown("Activate");
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
			
			if (UserSettings.CompletedScenes.Contains(component.SceneName))
				component.PreviewObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.5f);

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
			_allowedupdate = false;

			StartCoroutine(LoadScene(_current_portal_component.SceneName));
		}
	}

	private IEnumerator LoadScene(string name)
	{
		var current_color = TransitionImage.color;
		current_color.a = 0;

		var target_color = TransitionImage.color;
		target_color.a = 1;

		LeanTween.value(TransitionImage.gameObject, UpdateCanvasColor, current_color, target_color, RotationTime * 2);

		yield return new WaitForSeconds(RotationTime * 2 + 2);

        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

	private void UpdateTextAlpha(Color c)
	{
		PortalNameText.color = c;
		PortalDescriptionText.color = c;
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

        var show_color = new Color(1, 1, 1, 1);
        var hide_color = new Color(1, 1, 1, 0);
		LeanTween.value(gameObject, UpdateTextAlpha, show_color, hide_color, RotationTime * 0.5f);

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

			PortalNameText.text = SelectedObject.InterfaceName;
			PortalDescriptionText.text = SelectedObject.InterfaceDescription;
		}
		else { PortalNameText.text = DefaultName; PortalDescriptionText.text = DefaultDescription; }

        LeanTween.value(gameObject, UpdateTextAlpha, hide_color, show_color, RotationTime * 0.5f);

        yield return new WaitForSeconds(RotationTime * 0.5f);

		_current_portal_component = SelectedObject;
		_allowedupdate = true;
	}

	void UpdateCanvasColor(Color c)
	{
		TransitionImage.color = c;
	}
}
