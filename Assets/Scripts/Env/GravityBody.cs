using UnityEngine;

namespace Env {
	[RequireComponent(typeof(Rigidbody))]
	public class GravityBody : MonoBehaviour {
		private Rigidbody rb;

		private void Start() {
			rb = GetComponent<Rigidbody>();
			rb.useGravity = false;
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}

		private void FixedUpdate() {
			var closest = GetClosestAsteroid();
			if (closest == null || !closest.IsWithinGravityField(transform.position))
				return;

			var gravity = closest.GetGravity(transform.position);
			rb.AddForce(gravity);

			// Вращаем объект, чтобы "низ" смотрел на центр астероида
			var gravityUp = -closest.GetGravityDirection(transform.position);
			var targetRotation =
				Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
			transform.rotation =
				Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
		}

		private Asteroid GetClosestAsteroid() {
			Asteroid closest = null;
			var minDist = float.MaxValue;

			foreach (var a in Asteroid.All) {
				var dist = Vector3.Distance(transform.position, a.transform.position);
				if (dist < minDist) {
					closest = a;
					minDist = dist;
				}
			}

			return closest;
		}
	}
}