﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public int DeathY = -7;
	private Vector3 _startPoint
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

	}

	// Update is called once per frame
	void Update() {
		if (transform.position.y < DeathY) {
			Respawn();
		}
	}
}
