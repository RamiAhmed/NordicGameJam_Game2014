﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	[Range(0, 100)]
	public float PlayerHealth = 100f;

	public int DeathY = -7;

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
	}

	void Respawn() {
		_startPoint = _waypoints[Random.Range(0, _waypoints.Length)].transform.position;
		
		this.transform.position = _startPoint;
		this.transform.LookAt(_terrainCenterPoint);

		PlayerHealth = 100f;

	}

	// Update is called once per frame
	void Update() {
		if (transform.position.y < DeathY) {
			Respawn();
		}


		foreach (Collider hitCollider in Physics.OverlapSphere(this.transform.position, 5f)) {
			if (hitCollider.GetType() != typeof(TerrainCollider) && hitCollider.transform.root != this.transform.root) {
				Debug.Log("Colliding with: " + hitCollider);
				if (hitCollider.transform.root.gameObject.CompareTag("DynamicObstacle")) {
					takeDamage(hitCollider.transform.root.gameObject.GetComponent<DynamicObstacle>().HitDamageAmount);
				}
			}
		}
	}

	private void takeDamage(float damageAmount) {
		Debug.Log("Player takes " + damageAmount.ToString() + " damage.");
		PlayerHealth -= damageAmount; 

		if (PlayerHealth <= 0f) {
			Invoke("Respawn", 1.5f);
		}
	}

}


