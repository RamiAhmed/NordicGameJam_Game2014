using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour {

	public float KillY = -10f;

	// Use this for initialization
	void Start () {
		Debug.Log("Starting dynamic obstacle at: " + this.transform.position.ToString() + " at time: " + GameController.Instance.GameTime.ToString("F1"));
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y < KillY) {
			RemoveSelf();
		}
	}

	public void RemoveSelf() {
		GameController.Instance.DynamicObstacles.Remove(this);
		Destroy(this.gameObject);
	}
}
