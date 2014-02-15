using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public float PlayerScore = 0f;
	public float PlayerMultiplierIncreaseInterval = 30f;
	public int PlayerMultiplier = 1;

	public GUISkin PlayerFeedbackGUISkin = null;

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
	private float _lastMultiplierDecrease = 0f;

	public float ShowFeedbackDuration = 4f;
	private float _lastFeedback = 0f;
	private string _feedbackText = "";

	public float TimeAlive = 0f;

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

	private void increaseMultiplier() {
		if (GameController.Instance.GameTime - _lastMultiplierIncrease > 3f) {
			_lastMultiplierIncrease = GameController.Instance.GameTime;

			PlayerMultiplier++;

			PrintFeedback("Multiplier +" + PlayerMultiplier.ToString());

			GameController.Instance.AudioController.NextClip();
		}
	}

	private void decreaseMultiplier() {
		if (GameController.Instance.GameTime - _lastMultiplierDecrease > 3f) {
			_lastMultiplierDecrease = GameController.Instance.GameTime;

			PlayerMultiplier = PlayerMultiplier - 1 > 0 ? PlayerMultiplier - 1 : 1;

			PrintFeedback("Multiplier -" + PlayerMultiplier.ToString());

			GameController.Instance.AudioController.PreviousClip();
		}
	}

	void Respawn() {
		PrintFeedback("Rebooting...");

		_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
		
		this.transform.position = _startPoint;
		this.transform.LookAt(_terrainCenterPoint);

		PlayerHealth = 100f;
		IsDead = false;

		PlayerMultiplier = 1;
		PlayerScore = 0;

		TimeAlive = 0f;

		GameController.Instance.AudioController.ChangeClip(0);

		_lastMultiplierIncrease = 0f;
		_lastMultiplierDecrease = 0f;

	}

	void OnGUI() {
		if (_feedbackText != "") {
			if (PlayerFeedbackGUISkin != null) {
				if (GUI.skin != PlayerFeedbackGUISkin) {
					GUI.skin = PlayerFeedbackGUISkin;
				}
			}

			float width = 300f, height = 150f;
			GUI.Label(new Rect(Screen.width/3f, Screen.height/3f, width, height), _feedbackText);
		}
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

		TimeAlive += Time.deltaTime;

		if (TimeAlive - _lastMultiplierIncrease > PlayerMultiplierIncreaseInterval) {
			increaseMultiplier();
		}


		foreach (Collider hitCollider in Physics.OverlapSphere(this.transform.position, 5f)) {
			if (hitCollider.GetType() != typeof(TerrainCollider) && hitCollider.transform.root != this.transform.root) {
				Debug.Log("Colliding with: " + hitCollider);
				if (hitCollider.transform.root.gameObject.CompareTag("DynamicObstacle")) {
					DynamicObstacle dynObs = hitCollider.transform.root.gameObject.GetComponent<DynamicObstacle>();
					if (dynObs != null) {
						float damageAmount = dynObs.HitDamageAmount;
						switch (dynObs.Type) {
							case DynamicObstacle.ObstacleType.ENEMY:
								PrintFeedback("Malware detected!");
								damageAmount *= 2f; 
								decreaseMultiplier();
							break;

							case DynamicObstacle.ObstacleType.TARGET: 
								PrintFeedback("Cleaned up the system successfully.");
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
			PrintFeedback("System catastrophic crash!");

			IsDead = true;

			this.rigidbody.velocity = Vector3.zero;
			this.rigidbody.angularVelocity = Vector3.zero;

			Invoke("Respawn", 3f);
		}
	}


	public void PrintFeedback(string feedback) {
		if (GameController.Instance.GameTime - _lastFeedback > ShowFeedbackDuration) {
			_lastFeedback = GameController.Instance.GameTime;

			_feedbackText = feedback;

			Invoke("disableFeedback", ShowFeedbackDuration);
		}
	}

	private void disableFeedback() {
		if (_feedbackText != "") {
			_feedbackText = "";
		}
	}

}


