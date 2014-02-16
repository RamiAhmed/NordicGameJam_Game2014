using UnityEngine;
using System.Collections;

public abstract class Movement_Circle : MonoBehaviour
{
	// public editor variables
	public float MaxMoveSpeed = 10;
	public float Mass;
	public float MoveSpeed = 5;
	public float Deceleration = 5;
	public float TurnSpeed = 2.5f;

	// private class variables
	private const float deadZone = 0.25f;

	private GameController gameController;

	private Vector2 velocity = Vector2.zero;

	void Awake()
	{
		gameController = FindObjectOfType(typeof(GameController)) as GameController;
	}

	void Start()
	{
		Reset();
	}

	public void Reset()
	{
		float distanceToFloor = collider.bounds.extents.y;
		transform.position = -transform.position.normalized * (gameController.WorldRadius - distanceToFloor);
		transform.up = -transform.position;

		velocity = Vector2.zero;
	}

	// ref: http://www.gamasutra.com/blogs/JoshSutphin/20130416/190541/Doing_Thumbstick_Dead_Zones_Right.php
	public void Move(Vector2 input)
	{
		if (input.magnitude > deadZone)
		{
			// input ranges from -1..0..1 on each axis
			input = input.normalized * ((input.magnitude - deadZone) / (1 - deadZone));

			// integrate speed
			Vector2 acceleration = input * (MoveSpeed / Mass);
			velocity += acceleration * Time.deltaTime;

			velocity = Vector2.ClampMagnitude(velocity, MaxMoveSpeed);
		}
		else
		{
			// slow down
			velocity = Vector2.Lerp(velocity, Vector2.zero, Deceleration * Time.deltaTime);
		}

		transform.RotateAround(Vector3.zero, transform.right, velocity.y * Time.deltaTime); // move forward/backward
		transform.RotateAround(Vector3.zero, -transform.up, velocity.x * Time.deltaTime); // move right/left

	}
}
