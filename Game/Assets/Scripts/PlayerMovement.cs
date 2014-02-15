using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	// public editor variables
	public float FloorMoveSpeed = 10;
	//public float AirMoveSpeed = 2;
	public float Deceleration = 5;
	public float TurnSpeed = 2.5f;
	public float Gravity = 10;
	//public float MaxJumpTime = .5f;
	//public float JumpSpeed = 200;

	// private class variables

	private const float deadZone = 0.25f;
	//private float distanceToGround;

	//private bool grounded
	//{
	//	get
	//	{
	//		return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
	//	}
	//}

	//private bool jumping;
	//private float jumpTimer;

	private Camera playerCam;

	// Use this for initialization
	void Start()
	{
		playerCam = Camera.main;

		//distanceToGround = collider.bounds.extents.y;
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 movementDir = Vector3.zero;

		// ref: http://www.gamasutra.com/blogs/JoshSutphin/20130416/190541/Doing_Thumbstick_Dead_Zones_Right.php
		Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		if (input.magnitude > deadZone)
		{
			input = input.normalized * ((input.magnitude - deadZone) / (1 - deadZone));
			
			movementDir = playerCam.transform.right * input.x + playerCam.transform.forward * input.y;

			movementDir.y = 0;
			movementDir.Normalize();

			// turn to look forward
			transform.forward = Vector3.Lerp(transform.forward, movementDir, Time.deltaTime * TurnSpeed);

			// different speeds in air and on floor
			//if (grounded)
				rigidbody.AddForce(movementDir * FloorMoveSpeed);
			//else
			//	rigidbody.AddForce(movementDir * AirMoveSpeed);
		}
		else
		{
			// slow down when not pressing any directional buttons
			Vector3 v = rigidbody.velocity;
			v.x = Mathf.Lerp(v.x, 0, Time.deltaTime * Deceleration);
			v.z = Mathf.Lerp(v.z, 0, Time.deltaTime * Deceleration);
			rigidbody.velocity = v;

			// ignore collision-created turning
			rigidbody.angularVelocity = Vector3.zero;
		}

		// turn jump mode on
		//if (grounded && Input.GetButtonDown("Jump"))
		//{
		//	jumping = true;
		//	jumpTimer = MaxJumpTime;
		//}
		
		//// turn jump mode off
		//if (Input.GetButtonUp("Jump"))
		//{
		//	jumping = false;
		//	jumpTimer = 0;
		//}

		//if (jumping)
		//{
		//	jumpTimer -= Time.deltaTime;

		//	if (jumpTimer <= 0)
		//	{
		//		jumping = false;
		//		jumpTimer = 0;
		//	}

		//	Vector3 v = rigidbody.velocity;
		//	v.y = JumpSpeed * Time.deltaTime;
		//	rigidbody.velocity = v;
		//}
		//else if (!grounded) // && !jumping
		//{
		//	// add gravity only when not grounded and when not jumping
		//	rigidbody.AddForce(Vector3.down * Gravity);
		//}
	}
}
