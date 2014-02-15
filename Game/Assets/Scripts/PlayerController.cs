using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	[Range(0, 100)]
	public float PlayerHealth = 100f;

	public float PlayerRegenerationPerSecond = 0.5f;
	private float _lastRegen = 0f;

	public int DeathY = -7;

	public bool IsDead = false;

	private Vector3 _startPoint = Vector3.zero;
	private Vector3 _terrainCenterPoint = Vector3.zero;

	private GameObject[] _waypoints = null;

	// Use this for initialization
	void Start() {

		_waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;

		this.transform.position = _startPoint;

		Vector3 terrainSize = GameObject.FindGameObjectWithTag("Ground").GetComponent<Terrain>().terrainData.size;
		_terrainCenterPoint = new Vector3(terrainSize.x/2f, 0f, terrainSize.z/2f);

		this.transform.LookAt(_terrainCenterPoint);

		InvokeRepeating("regenerate", 1f, 1f);
	}

	private void regenerate() {
		PlayerHealth = PlayerHealth + PlayerRegenerationPerSecond > 100f ? 100f : PlayerHealth + PlayerRegenerationPerSecond;
	}

	void Respawn() {
		_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
		
		this.transform.position = _startPoint;
		this.transform.LookAt(_terrainCenterPoint);

		PlayerHealth = 100f;
		IsDead = false;

	}

	// Update is called once per frame
	void Update() {
		if (IsDead) {
			return;
		}


		if (transform.position.y < DeathY) {
			Respawn();
		}


		foreach (Collider hitCollider in Physics.OverlapSphere(this.transform.position, 5f)) {
			if (hitCollider.GetType() != typeof(TerrainCollider) && hitCollider.transform.root != this.transform.root) {
				Debug.Log("Colliding with: " + hitCollider);
				if (hitCollider.transform.root.gameObject.CompareTag("DynamicObstacle")) {
					DynamicObstacle dynObs = hitCollider.transform.root.gameObject.GetComponent<DynamicObstacle>();
					if (dynObs != null) {
						float damageAmount = dynObs.HitDamageAmount;
						switch (dynObs.Type) {
							case DynamicObstacle.ObstacleType.ENEMY: damageAmount *= 2f; break;
							case DynamicObstacle.ObstacleType.TARGET: damageAmount *= -1f; break;
						}

						takeDamage(damageAmount);
					}
				}
			}
		}

	}

	private void takeDamage(float damageAmount) {
		Debug.Log("Player takes " + damageAmount.ToString() + " damage.");
		PlayerHealth -= damageAmount;
		if (PlayerHealth > 100f) 
			PlayerHealth = 100f;

		if (PlayerHealth <= 0f) {
			IsDead = true;

			this.rigidbody.velocity = Vector3.zero;
			this.rigidbody.angularVelocity = Vector3.zero;

			Invoke("Respawn", 1.5f);
		}
	}

}


