using UnityEngine;
using System.Collections;

public class PlayerMovement : Movement_Circle
{

	protected Animator animator = null;

	void Start() {
		animator = GetComponentInChildren<Animator>();
	}

	void Update() {
		if (!this.transform.root.GetComponent<PlayerController>().IsDead) {
			Vector3 movementDir = Vector3.zero;

			Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			Move(input);

			if (input.magnitude > 0.1f)
			{
				if (animator)
				{
					animator.SetBool("isRunning", true);
				}
			}
			else
			{
				if (animator)
				{
					animator.SetBool("isRunning", false);
				}
			}

			
		}
	}
}
