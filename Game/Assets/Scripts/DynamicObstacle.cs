using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour {

	public float MovementSpeed = 10f;
	
	public float SpawnRadiusFromPlayer = 50f;

	public float MinDistanceFromOtherObstacles = 15f;

	public float MinMoveDistance = 100f;

	public float KillY = -15f;

	private PlayerController _player = null;

	private Vector3 _startPoint = Vector3.zero, _endPoint = Vector3.zero;

	// Use this for initialization
	void Start () {
		Debug.Log("Starting dynamic obstacle at time: " + GameController.Instance.GameTime.ToString("F1"));

		_player = GameController.Instance.Player.GetComponent<PlayerController>();
		if (_player == null) {
			Debug.LogError(this.ToString() + " could not find the player through the GameController: " + GameController.Instance.ToString());
		}

		Initialize();
	}

	void Initialize() {		

		GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

		do {
			_startPoint = waypoints[Random.Range(0, waypoints.Length)].transform.position;

		} while (Vector3.Distance(_startPoint, this.transform.position) < SpawnRadiusFromPlayer && !isAnyObstacleTooNear());

		do {
			_endPoint = waypoints[Random.Range(0, waypoints.Length)].transform.position;

		} while (Vector3.Distance(_startPoint, _endPoint) < MinMoveDistance);

		this.transform.position = _startPoint;

		GameController.Instance.DynamicObstacles.Add(this);
	}

	private bool isAnyObstacleTooNear() {
		bool result = false;
		foreach (DynamicObstacle go in GameController.Instance.DynamicObstacles) {
			if (Vector3.Distance(go.transform.position, this.transform.position) < MinDistanceFromOtherObstacles) {
				result = true;
				break;
			}
		}

		return result;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y < KillY) {
			RemoveSelf();
		}
	}

	void FixedUpdate() {


	}

	public void RemoveSelf() {
		GameController.Instance.DynamicObstacles.Remove(this);
		Destroy(this.gameObject);

		Debug.Log(this.ToString() + " is self-removing at time: " + GameController.Instance.GameTime);

		GameController.Instance.SpawnDynamicObstacle();
	}

	private float getTerrainHeightAtPosition(Vector3 position) {
		float objectHeight = GetComponentInChildren<Collider>().bounds.extents.y / 2f;
		return Terrain.activeTerrain.SampleHeight(position) + objectHeight;
	}
}
