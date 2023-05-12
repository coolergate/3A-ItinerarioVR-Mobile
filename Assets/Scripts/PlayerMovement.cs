using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	CharacterController _body;

	float movementSpeed = 10.0f;

	// Start is called before the first frame update
	void Start()
	{
		_body = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		float Horizontal = Input.GetAxis("Horizontal");
		float Vertical = Input.GetAxis("Vertical");

		_body.Move(new Vector3(Horizontal, -10, Vertical) * movementSpeed * Time.deltaTime);
	}
}
