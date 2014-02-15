using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject Player = null;
	private PlayerController _player;

	public int InitialAmountOfDynamicObstacles = 5;

	public List<DynamicObstacle> DynamicObstacles = null;

	public float GameTime = 0f;
	public int SpawnInterval = 15;

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
				//Invoke("SpawnDynamicObstacle", (float)i * 0.1f);
				SpawnDynamicObstacle();
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
	
	// Update is called once per frame
	void Update () {
		if (!_initialized || _player == null) {
			return;
		}


		// Iterate the game time
		GameTime += Time.deltaTime;


		if (Mathf.RoundToInt(GameTime) % SpawnInterval == 0) {
			SpawnDynamicObstacle();
		}	


	}

	void OnGUI() {
		GUI.Box (new Rect(5f, 5f, 100f, 50f), new GUIContent("Time: " + GameTime.ToString("F1")));
	}

	public void SpawnDynamicObstacle() {
		Debug.Log("Spawning dynamic obstacle. GameTime: " + GameTime.ToString("F1"));

		Instantiate(Resources.Load("DynamicObstacle"));

	}
}
