using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour {

	public float KillY = -10f;

	private PlayerController _player = null;

	// Use this for initialization
	void Start () {
		Debug.Log("Starting dynamic obstacle at: " + this.transform.position.ToString() + " at time: " + GameController.Instance.GameTime.ToString("F1"));

		_player = GameController.Instance.Player.GetComponent<PlayerController>();
		if (_player == null) {
			Debug.LogError(this.ToString() + " could not find the player through the GameController: " + GameController.Instance.ToString());
		}
	}

	void Initialize() {
		GameController.Instance.DynamicObstacles.Add(this);
		
		float randValue = Random.value;
		this.transform.position = _player.transform.position + (new Vector3(randValue, 0f, 1 - randValue) * GameController.Instance.SpawnRadiusFromPlayer);
		
		if (Vector3.Distance(_player.transform.position, this.transform.position) < GameController.Instance.SpawnRadiusFromPlayer) {
			this.RemoveSelf(); // Remove if we're within the allowed radius
			Debug.LogWarning("Removing " + this.ToString() + " because it is too close to the player. Distance calculated: " + Vector3.Distance(_player.transform.position, this.transform.position).ToString());
		}
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
	}
}
