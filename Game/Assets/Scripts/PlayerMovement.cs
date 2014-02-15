using UnityEngine;
using System.Collections;

public class PlayerMovement : Movement_Circle
{
	void Update() {
		if (!this.transform.root.GetComponent<PlayerController>().IsDead) {
			Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
		}
	}
}
