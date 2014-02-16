using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
	public float FloorMoveSpeed = 10;
	public float Deceleration = 5;
	public float TurnSpeed = 2.5f;

	private const float deadZone = 0.25f;

	protected Animator animator = null;

	void Start() {
		animator = GetComponentInChildren<Animator>();
	}

	void Update() {
		if (!this.transform.root.GetComponent<PlayerController>().IsDead) {
			Vector3 movementDir = Vector3.zero;

			Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			
			if (input.magnitude > deadZone)	{
				input = input.normalized * ((input.magnitude - deadZone) / (1 - deadZone));
				
				movementDir = transform.right * input.x + transform.forward * input.y;
				
				movementDir.y = 0;
				movementDir.Normalize();
				
				// turn to look forward
				transform.forward = Vector3.Lerp(transform.forward, movementDir, Time.deltaTime * TurnSpeed);
				
				// move
				rigidbody.AddForce(movementDir * FloorMoveSpeed);

				if (animator) {
					animator.SetBool("isRunning", true);
				}

			}
			else {
				// slow down when not pressing any directional buttons
				Vector3 v = rigidbody.velocity;
				v.x = Mathf.Lerp(v.x, 0, Time.deltaTime * Deceleration);
				v.z = Mathf.Lerp(v.z, 0, Time.deltaTime * Deceleration);
				rigidbody.velocity = v;
				
				// ignore collision-created turning
				rigidbody.angularVelocity = Vector3.zero;

				if (rigidbody.velocity.magnitude <= 0.1f) {
					if (animator) {
						animator.SetBool("isRunning", false);
					}
				}
			}
		}
	}
}
