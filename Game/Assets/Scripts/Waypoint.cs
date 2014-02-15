using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, 0.7f);
		Gizmos.DrawSphere(transform.position, 2f);
	}
}
