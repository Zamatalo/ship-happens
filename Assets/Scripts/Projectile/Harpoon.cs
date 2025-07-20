using UnityEngine;

namespace Projectile {
	public class Harpoon : MonoBehaviour {
		public float pullSpeed;
		public bool hit;
		private float _force;
		private LineRenderer _lr;
		private Transform _notActualOrigin;
		private Transform _origin;
		private Rigidbody _rb;

		private void Start() {
			_rb = GetComponent<Rigidbody>();
			_lr = GetComponent<LineRenderer>();

			_rb.AddForce(_origin.forward * _force);
		}

		private void Update() {
			_lr.SetPosition(0, _notActualOrigin.position);
			_lr.SetPosition(1, transform.position);
		}

		private void OnTriggerEnter(Collider other) {
			if (!other.TryGetComponent(out Loot.Loot _)) return;

			_rb.constraints = RigidbodyConstraints.FreezeAll;
			transform.SetParent(other.transform);
			transform.localPosition = Vector3.zero;
			hit = true;
		}

		public void Init(Transform notActualOrigin, Transform origin, float force) {
			_notActualOrigin = notActualOrigin;
			_origin = origin;
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