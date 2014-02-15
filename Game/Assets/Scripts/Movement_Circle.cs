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

	/// <summary>
	/// Spherical coordinates,
	/// x - angles on left/right axis
	/// y - angles on up/down axis
	/// z/radius - distance from center
	/// </summary>
	private Vector2 velocity = Vector2.zero;
	private Vector2 position = Vector2.zero;

	void Awake()
	{
		gameController = FindObjectOfType(typeof(GameController)) as GameController;
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
		}
		else
		{
			// slow down
		}

		position += velocity * Time.deltaTime;

		// set real world coords

	}
}
