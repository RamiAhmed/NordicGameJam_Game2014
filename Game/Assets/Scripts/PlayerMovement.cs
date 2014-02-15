using UnityEngine;
using System.Collections;

public class PlayerMovement : Movement
{
	void Update()
	{
		Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
	}
}
