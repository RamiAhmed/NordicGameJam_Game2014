using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	public GUISkin MenuGUISkin = null;

	public Texture2D MenuBackground;
	
	private float _screenWidth = 0f, _screenHeight = 0f;


	void Start () {
		_screenWidth = Screen.width;
		_screenHeight = Screen.height;
	}

	void OnGUI() {
		if (MenuGUISkin != null) {
			if (GUI.skin != MenuGUISkin) {
				GUI.skin = MenuGUISkin;
			}
		}

		if (MenuBackground != null)
			GUI.DrawTexture(new Rect(0f, 0f, _screenWidth, _screenHeight), MenuBackground);

		float y = _screenHeight/4f;

		if (GUI.Button(new Rect(_screenWidth/2f, y, 300f, 100f), new GUIContent("Socialize!", "Click this button to start playing"))) {
			Application.LoadLevel("test_scene_1");
		}

		if (GUI.Button(new Rect(_screenWidth/2f, y + 150f, 300f, 100f), new GUIContent("Crash the System", "Click this button to exit the game"))) {
			if (Application.isEditor) {
				Debug.Log("Exiting the game (theoretically)");
			}
			else {
				Application.Quit();
			}
		}

		if (GUI.tooltip != "") {
			GUI.Box(new Rect(Input.mousePosition.x - 250f, _screenHeight - Input.mousePosition.y - 50f, 180f, 150f), GUI.tooltip);
		}
	}

}
