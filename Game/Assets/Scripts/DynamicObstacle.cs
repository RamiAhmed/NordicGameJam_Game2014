using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour {

	public float MovementSpeed = 10f;
	
	public float SpawnRadiusFromPlayer = 50f;

	public float MaxMoveDistance = 100f;

	public float KillY = -15f;

	private PlayerController _player = null;

	private Vector3 _startPoint = Vector3.zero, _endPoint = Vector3.zero;
	private float _moveDuration = 0f, _startTime = 0f;

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
		Vector3 startPos = _player.transform.position + (Random.onUnitSphere * SpawnRadiusFromPlayer);
		startPos.y = getTerrainHeightAtPosition(startPos);
		this.transform.position = startPos;

		_startPoint = this.transform.position;

		_endPoint = this.transform.position + (Random.onUnitSphere * MaxMoveDistance);
		_endPoint.y = getTerrainHeightAtPosition(_endPoint);

		_moveDuration = Vector3.Distance(_startPoint, _endPoint) / MovementSpeed;

		_startTime = GameController.Instance.GameTime;

		GameController.Instance.DynamicObstacles.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y < KillY) {
			RemoveSelf();
		}
	}

	void FixedUpdate() {
		float durationProgress = (GameController.Instance.GameTime - _startTime) / _moveDuration;
		this.transform.position = Vector3.Lerp(_startPoint, _endPoint, durationProgress);

		if (durationProgress >= 1f) {
			RemoveSelf();
		}

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
