using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicObstacle : MonoBehaviour {

	public enum ObstacleType { NEUTRAL, ENEMY, TARGET };

	public ObstacleType Type = ObstacleType.NEUTRAL;

	public float HitDamageAmount = 0.5f;

	public float MovementSpeedScaleFactor = 0.01f;

	public float MaxMovementSpeed = 25f;

	public float MovementSpeed = 5f;

	public float Deceleration = 10f;

	public float TurnSpeed = 2.5f;

	public float SpawnRadiusFromPlayer = 40f;

	public float MinDistanceFromOtherObstacles = 15f;

	public float MinMoveDistance = 40f;

	public float RestartDistance = 5f;
	public float SlowingDistance = 20f;

	public float KillY = -15f;

	private PlayerController _player = null;

	private Vector3 _startPoint = Vector3.zero, _endPoint = Vector3.zero;

	private bool _initialized = false;

	private GameObject[] _waypoints = null;

	private Animator _animator = null;
	
	void Start () {
		_player = GameController.Instance.Player.GetComponent<PlayerController>();
		if (_player == null) {
			Debug.LogError(this.ToString() + " could not find the player through the GameController: " + GameController.Instance.ToString());
		}

		Initialize();
	}

	void Initialize() {		
		_waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

		int failSafe = 0;
		do {
			_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
			
			failSafe++;
			if (failSafe > 100) {
				Debug.LogError("Could not find a valid start point before the time ran out");
				break;
			}
			
		} while (Vector3.Distance(_startPoint, this.transform.position) < SpawnRadiusFromPlayer || isAnyObstacleTooNear(this.transform.position));

		_endPoint = getRandomEndPoint();

		this.transform.position = _startPoint;

		_animator = this.GetComponentInChildren<Animator>();

		GameController.Instance.DynamicObstacles.Add(this);

		_initialized = true;
	}
	
	private bool isAnyObstacleTooNear(Vector3 comparisonPosition) {
		bool result = false;
		foreach (DynamicObstacle go in GameController.Instance.DynamicObstacles) {
			if (Vector3.Distance(go.transform.position, comparisonPosition) < MinDistanceFromOtherObstacles) {
				result = true;
				break;
			}
		}
		
		return result;
	}
	
	private Vector3 getRandomEndPoint() 
	{
		int failSafe = 0;
		
		Vector3 endPoint = Vector3.zero;
		do {
			endPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
			
			failSafe++;
			
			if (failSafe >= 1000) {
				Debug.LogError("Could not find a valid endPoint before the time ran out");
				break;
			}
			
		} while (Vector3.Distance(_startPoint, endPoint) < MinMoveDistance || isAnyObstacleTooNear(endPoint));

		return endPoint;
	}

	public void GetNewDirection() {
		_endPoint = getRandomEndPoint();
	}

	void Update () {
		if (!_initialized) {
			return;
		}

		if (this.transform.position.y < KillY) {
			RemoveSelf();
		}

		Vector3 movementDir3 = _endPoint - _startPoint;

		if (Vector3.Distance(this.transform.position, _endPoint) >= SlowingDistance) {
			Vector3 movementDirection = (_endPoint - _startPoint).normalized;
			movementDirection.y = 0f;

			this.transform.forward = Vector3.Lerp(this.transform.forward, movementDirection, Time.deltaTime * TurnSpeed);

			float speed = (MovementSpeed + (_player.PlayerMultiplier * 0.1f)) * 2f;
			this.rigidbody.AddForce(movementDirection * speed);

			this.rigidbody.velocity = Vector3.ClampMagnitude(this.rigidbody.velocity, MaxMovementSpeed);

			if (_animator) {
				_animator.SetBool("isRunning", true);
			}
		}
		else {
			Vector3 velocity = this.rigidbody.velocity;
			velocity.x = Mathf.Lerp(velocity.x, 0f, Time.deltaTime * Deceleration);
			velocity.z = Mathf.Lerp(velocity.z, 0f, Time.deltaTime * Deceleration);
			this.rigidbody.velocity = velocity;

			this.rigidbody.angularVelocity = Vector3.zero;

			if (this.rigidbody.velocity.magnitude < 0.1f) {
				_startPoint = _endPoint;
				_endPoint = getRandomEndPoint();

				if (_animator) {
					_animator.SetBool("isRunning", false);
				}
			}
		}
		/*
		switch (this.Type) {
			case ObstacleType.ENEMY: changeMaterialColor(Color.red); break;
			case ObstacleType.NEUTRAL: changeMaterialColor(Color.white); break;
			case ObstacleType.TARGET: changeMaterialColor(Color.green); break;
		}*/

		MovementSpeed += (Time.deltaTime * MovementSpeedScaleFactor);


	}
	/*
	private void changeMaterialColor(Color newColor) {
		foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
			foreach (Material mat in rend.materials) {
				if (mat.color != newColor)
					mat.color = newColor;
			}
		}
	}*/


	public void RemoveSelf() {
		if (GameController.Instance.DynamicObstacles.Contains(this)) {
			GameController.Instance.Remove(this);
		}

		Destroy(this.gameObject);

		GameController.Instance.SpawnDynamicObstacle();
	}

}
