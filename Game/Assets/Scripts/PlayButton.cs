using UnityEngine;
using System.Collections;

public class PlayButton : MonoBehaviour
{
	void OnMouseDown()
	{
		Application.LoadLevel(1);
	}
}
