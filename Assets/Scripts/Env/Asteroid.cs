using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
	public static List<Asteroid> All = new();
	public float gravity = 9.8f;
	public float gravityRadius = 50f;

	private void OnEnable() {
		All.Add(this);
	}

	private void OnDisable() {
		All.Remove(this);
	}

	private void OnDrawGizmosSelected() // Not important just for me and my life to make it easier
	{
		Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
		Gizmos.DrawSphere(transform.position, gravityRadius);
	}

	public bool IsWithinGravityField(Vector3 position) {
		return Vector3.Distance(transform.position, position) <= gravityRadius;
	}

	public Vector3 GetGravityDirection(Vector3 position) {
		return (transform.position - position).normalized;
	}

	public Vector3 GetGravity(Vector3 position) {
		if (!IsWithinGravityField(position))
			return Vector3.zero;

		return GetGravityDirection(position) * gravity;
	}
}