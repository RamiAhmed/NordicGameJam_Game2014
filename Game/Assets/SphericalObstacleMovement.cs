using UnityEngine;
using System.Collections;

public class SphericalObstacleMovement : Movement_Circle
{
	DynamicObstacle obst;

	void Awake()
	{
		obst = GetComponent<DynamicObstacle>();
	}

	// Update is called once per frame
	void Update()
	{
	}
}
