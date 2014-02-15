using UnityEngine;
using System.Collections;

public class ExitButton : MonoBehaviour
{

	void OnMouseDown()
	{
		Debug.Log("Exit.");
		Application.Quit();
	}
}
