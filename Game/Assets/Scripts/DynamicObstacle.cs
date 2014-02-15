using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour {

	public enum ObstacleType { NEUTRAL, ENEMY, TARGET };

	public ObstacleType Type = ObstacleType.NEUTRAL;

	public float HitDamageAmount = 5f;

	public float MovementSpeed = 10f;

	public float Deceleration = 5f;

	public float TurnSpeed = 2.5f;

	public float SpawnRadiusFromPlayer = 50f;

	public float MinDistanceFromOtherObstacles = 15f;

	public float MinMoveDistance = 50f;

	public float RestartDistance = 5f;
	public float SlowingDistance = 15f;

	public float KillY = -15f;

	private PlayerController _player = null;

	private Vector3 _startPoint = Vector3.zero, _endPoint = Vector3.zero;

	private bool _initialized = false;

	private GameObject[] _waypoints = null;

	// Use this for initialization
	void Start () {
		//Debug.Log("Starting dynamic obstacle at time: " + GameController.Instance.GameTime.ToString("F1"));

		_player = GameController.Instance.Player.GetComponent<PlayerController>();
		if (_player == null) {
			Debug.LogError(this.ToString() + " could not find the player through the GameController: " + GameController.Instance.ToString());
		}

		Initialize();
	}

	void Initialize() {		

		// Cache all the waypoints in a local variable
		_waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

		// Keep generating a random starting position untill we find one that is far enough away from the player and other dynamic obstacles
		int failSafe = 0;
		do {
			_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
			
			failSafe++;
			if (failSafe > 100) {
				Debug.LogError("Could not find a valid start point before the time ran out");
				break;
			}
			
		} while (Vector3.Distance(_startPoint, this.transform.position) < SpawnRadiusFromPlayer || isAnyObstacleTooNear(this.transform.position));

		// Get a random end point (same method as start point)
		_endPoint = getRandomEndPoint();

		// Move the object to the start position
		this.transform.position = _startPoint;

		//Debug.Log(this.ToString() + ". StartPoint: " + _startPoint.ToString() + ". EndPoint: " + _endPoint.ToString());

		// Add the object to the game controller's list of dynamic obstacles
		GameController.Instance.DynamicObstacles.Add(this);

		_initialized = true;

		// TODO change visualization of type
	}

	// Returns true if any other dynamic obstacle is within MinDistanceFromOtherObstacles
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

	// Returns a random end point based on the cached waypoints
	private Vector3 getRandomEndPoint() 
	{
		int failSafe = 0;
		
		Vector3 endPoint = Vector3.zero;
		do
		{
			endPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
			
			failSafe++;
			
			if (failSafe >= 1000) {
				Debug.LogError("Could not find a valid endPoint before the time ran out");
				break;
			}
			
		} while (Vector3.Distance(_startPoint, endPoint) < MinMoveDistance || isAnyObstacleTooNear(endPoint));
		
		
		return endPoint;
	}
	
	// Update is called once per frame
	void Update () {
		if (!_initialized) {
			return;
		}

		// Remove ourselves if we drop below the KillY
		if (this.transform.position.y < KillY) {
			RemoveSelf();
		}

		Vector3 movementDir3 = _endPoint - _startPoint;
		// If we're within reach of the end point, restart 
		if (Vector3.Distance(this.transform.position, _endPoint) < RestartDistance) {
			_startPoint = _endPoint;
		Vector2 movementDir2 = new Vector2(movementDir3.x, movementDir3.z).normalized;
		//Debug.Log("Movement direction: " + movementDir2.ToString());

		}
		else {

			if (Vector3.Distance(this.transform.position, _endPoint) >= SlowingDistance) {
				Vector3 movementDirection = (_endPoint - _startPoint).normalized;
				movementDirection.y = 0f;

				this.transform.forward = Vector3.Lerp(this.transform.forward, movementDirection, Time.deltaTime * TurnSpeed);

				this.rigidbody.AddForce(movementDirection * MovementSpeed);
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
				}
			}

		}
	}

	void FixedUpdate() {


	}

	public void RemoveSelf() {
		if (GameController.Instance.DynamicObstacles.Contains(this)) {
			GameController.Instance.Remove(this);
		}

		Destroy(this.gameObject);

		//Debug.Log(this.ToString() + " is self-removing at time: " + GameController.Instance.GameTime);

		GameController.Instance.SpawnDynamicObstacle();
	}

	/*
	private float getTerrainHeightAtPosition(Vector3 position) {
		float objectHeight = GetComponentInChildren<Collider>().bounds.extents.y / 2f;
		return Terrain.activeTerrain.SampleHeight(position) + objectHeight;
	}
	*/
}
