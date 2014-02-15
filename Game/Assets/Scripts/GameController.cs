﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject Player = null;
	private PlayerController _player;

	public int InitialAmountOfDynamicObstacles = 5;

	public List<DynamicObstacle> DynamicObstacles = null;

	public float GameTime = 0f;
	public int SpawnInterval = 15;

	public bool SpawnPeriodically = true;

	public int MaxTargets = 1;
	public int MaxEnemiesSpawned = 5;
	private int targetsSpawned = 0;
	private int enemiesSpawned = 0;

	#region GameController Singleton Pattern
	public static string PrefabPathAndName = "GameController";

	private static GameController _instance = null;
	private static bool _initialized = false;

	public static GameController Instance {
		get {
			if (_instance == null) {
				_instance = (Instantiate(Resources.Load(PrefabPathAndName)) as GameObject).GetComponent<GameController>();
				if (_instance != null) {
					_instance.Initialize();
					return _instance;
				}
				else {
					Debug.LogError("Could not instantiate GameController object. Looking at path and name:" + PrefabPathAndName);
					return null;
				}
			}
			else if (_instance != null) {
				return _instance;
			}
			else {
				Debug.LogError("Could not find the GameController. Looked at path: " + PrefabPathAndName);
				return null;
			}
		}
	}

	void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			Debug.Log("Removing duplicated GameController Singleton");
		}
		else {
			Debug.Log("GameController awaking");
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Starting the GameController");
		Initialize();
	}
	#endregion

	void Initialize() {
		if (!_initialized) {
			_initialized = true;
			Debug.Log("GameController initializing");

			DynamicObstacles = new List<DynamicObstacle>();

			for (int i = 0; i < InitialAmountOfDynamicObstacles; i++) {
				Invoke("SpawnDynamicObstacle", 3f + (float)i * 0.1f);
			}

			if (SpawnPeriodically) {
				InvokeRepeating("SpawnDynamicObstacle", 15f, (float)SpawnInterval);
			}

			if (Player != null) {
				_player = Player.GetComponent<PlayerController>();
				if (_player == null) {
					Debug.LogError("Could not find PlayerController component on Player: " + Player.ToString());
				}
			}
			else {
				Player = GameObject.FindGameObjectWithTag("Player");
				if (Player != null) {
					_player = Player.GetComponent<PlayerController>();
					if (_player == null) {
						Debug.LogError("Could not find PlayerController component on Player: " + Player.ToString());
					}
				}
				else {
					Debug.LogError("Player oject has not been set on GameController and could not be automatically found");
				}
			}
		}
	}

	public void Remove(DynamicObstacle obstacle)
	{
		DynamicObstacles.Remove(obstacle);
		if (obstacle.Type == DynamicObstacle.ObstacleType.ENEMY)
			enemiesSpawned--;
		else if (obstacle.Type == DynamicObstacle.ObstacleType.TARGET)
			targetsSpawned--;
	}
	
	// Update is called once per frame
	void Update () {
		if (!_initialized || _player == null) {
			return;
		}


		// Iterate the game time
		GameTime += Time.deltaTime;

	}

	void OnGUI() {
		string feedback = "Time: " + GameTime.ToString("F1");
		feedback += "\nObstacle count: " + DynamicObstacles.Count.ToString();
		feedback += "\nPlayer health: " + _player.PlayerHealth;
		
		GUI.Box (new Rect(5f, 5f, 200f, 100f), new GUIContent(feedback));
	}

	public void SpawnDynamicObstacle() {
		Debug.Log("Spawning dynamic obstacle. GameTime: " + GameTime.ToString("F1"));

		GameObject obst = Instantiate(Resources.Load("DynamicObstacle")) as GameObject;
		DynamicObstacle o = obst.GetComponent<DynamicObstacle>();
		
		if (targetsSpawned < MaxTargets)
		{
			o.Type = DynamicObstacle.ObstacleType.TARGET;
			targetsSpawned++;
		}

		if (enemiesSpawned < MaxEnemiesSpawned)
		{
			o.Type = DynamicObstacle.ObstacleType.ENEMY;
			enemiesSpawned++;
		}
	}
}
