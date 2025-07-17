using UnityEngine;

namespace Projectile {
	public class Harpoon : MonoBehaviour {
		public float pullSpeed;
		public bool hit;
		private Vector3 _dir;
		private float _force;
		private LineRenderer _lr;
		private Transform _origin;
		private Rigidbody _rb;

		private void Start() {
			_rb = GetComponent<Rigidbody>();
			_lr = GetComponent<LineRenderer>();

			_rb.AddForce(_dir * _force);
		}

		private void Update() {
			_lr.SetPosition(0, _origin.position);
			_lr.SetPosition(1, transform.position);
		}

		private void OnTriggerEnter(Collider other) {
			if (!other.TryGetComponent(out Loot.Loot loot)) return;

			_rb.constraints = RigidbodyConstraints.FreezeAll;
			transform.SetParent(other.transform);
			hit = true;
		}

		public void Init(Transform origin, Vector3 dir, float force) {
			_origin = origin;
			_dir = dir;
			_force = force;
		}

		public void Pull() {
			if (hit) {
				transform.parent.position = Vector3.MoveTowards(
					transform.parent.position,
					_origin.position,
					pullSpeed * Time.fixedDeltaTime
				);
				return;
			}

			transform.position = Vector3.MoveTowards(
				transform.position,
				_origin.position,
				pullSpeed * Time.fixedDeltaTime
			);
		}
	}
}