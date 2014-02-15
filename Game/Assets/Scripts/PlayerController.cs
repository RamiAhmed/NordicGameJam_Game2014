using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public float PlayerScore = 0f;
	public float PlayerMultiplierIncreaseInterval = 30f;
	public int PlayerMultiplier = 1;

	[Range(0, 100)]
	public float PlayerHealth = 100f;

	public float PlayerRegenerationPerSecond = 0.5f;
	private float _lastRegen = 0f;

	public int DeathY = -7;

	public bool IsDead = false;

	private Vector3 _startPoint = Vector3.zero;
	private Vector3 _terrainCenterPoint = Vector3.zero;

	private GameObject[] _waypoints = null;

	private float _lastMultiplierIncrease = 0f;

	// Use this for initialization
	void Start() {

		_waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;

		this.transform.position = _startPoint;

		Vector3 terrainSize = GameObject.FindGameObjectWithTag("Ground").GetComponent<Terrain>().terrainData.size;
		_terrainCenterPoint = new Vector3(terrainSize.x/2f, 0f, terrainSize.z/2f);

		this.transform.LookAt(_terrainCenterPoint);

		InvokeRepeating("regenerate", 1f, 1f);

		InvokeRepeating("increaseMultiplier", PlayerMultiplierIncreaseInterval, PlayerMultiplierIncreaseInterval);
	}

	private void regenerate() {
		PlayerHealth = PlayerHealth + PlayerRegenerationPerSecond > 100f ? 100f : PlayerHealth + PlayerRegenerationPerSecond;
	}

	private void increaseMultiplier() {
		if (GameController.Instance.GameTime - _lastMultiplierIncrease > 3f) {
			_lastMultiplierIncrease = GameController.Instance.GameTime;

			PlayerMultiplier++;
		}
	}

	void Respawn() {
		_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
		
		this.transform.position = _startPoint;
		this.transform.LookAt(_terrainCenterPoint);

		PlayerHealth = 100f;
		IsDead = false;

		PlayerMultiplier = 1;
		PlayerScore = 0;

	}

	// Update is called once per frame
	void Update() {
		if (IsDead) {
			return;
		}


		if (transform.position.y < DeathY) {
			Respawn();
		}


		PlayerScore += (Time.deltaTime * PlayerMultiplier);


		foreach (Collider hitCollider in Physics.OverlapSphere(this.transform.position, 5f)) {
			if (hitCollider.GetType() != typeof(TerrainCollider) && hitCollider.transform.root != this.transform.root) {
				Debug.Log("Colliding with: " + hitCollider);
				if (hitCollider.transform.root.gameObject.CompareTag("DynamicObstacle")) {
					DynamicObstacle dynObs = hitCollider.transform.root.gameObject.GetComponent<DynamicObstacle>();
					if (dynObs != null) {
						float damageAmount = dynObs.HitDamageAmount;
						switch (dynObs.Type) {
						case DynamicObstacle.ObstacleType.ENEMY: 
							damageAmount *= 2f; 
							PlayerMultiplier = PlayerMultiplier - 1 > 0 ? PlayerMultiplier - 1 : 1; 
							break;

							case DynamicObstacle.ObstacleType.TARGET: 
								damageAmount *= -1f; 
								increaseMultiplier();
								break;
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


