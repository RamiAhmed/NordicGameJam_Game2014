using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public abstract class Movement : MonoBehaviour
{
	// public editor variables
	public float FloorMoveSpeed = 10;
	public float Deceleration = 5;
	public float TurnSpeed = 2.5f;
	public float Gravity = 10;

	// private class variables
	private const float deadZone = 0.25f;

	// ref: http://www.gamasutra.com/blogs/JoshSutphin/20130416/190541/Doing_Thumbstick_Dead_Zones_Right.php
	public void Move(Vector2 input)
	{
		Vector3 movementDir = Vector3.zero;

		if (input.magnitude > deadZone)
		{
			input = input.normalized * ((input.magnitude - deadZone) / (1 - deadZone));

			movementDir = transform.right * input.x + transform.forward * input.y;

			movementDir.y = 0;
			movementDir.Normalize();

			// turn to look forward
			transform.forward = Vector3.Lerp(transform.forward, movementDir, Time.deltaTime * TurnSpeed);

			// move
			rigidbody.AddForce(movementDir * FloorMoveSpeed);
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
	}
}
