using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public int DeathY = -7;
	private Vector3 startPos;
	private Vector3 startRotEuler;

	// Use this for initialization
	void Start()
	{
		startPos = transform.position;
		startRotEuler = transform.rotation.eulerAngles;
	}

	void Respawn()
	{
		transform.position = startPos;
		transform.rotation = Quaternion.Euler(startRotEuler);
	}

	// Update is called once per frame
	void Update()
	{
		if (transform.position.y < DeathY)
			Respawn();
	}
}
