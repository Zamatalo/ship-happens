using UnityEngine;

namespace Projectile {
	public class Harpoon : MonoBehaviour {
		[Header("Settings")] public bool hit;
		public float pullSpeed = 5f;

		[Header("Rope Settings")] public float ropeWidth = 0.02f;
		public Material ropeMaterial;
		public int ropeSegments = 6;
		public float ropeSag = 0.2f;
		public float ropeSpring = 20f;
		public float ropeDamping = 5f;
		private Vector3 _attachmentPoint;
		private bool _isPulling;
		private LineRenderer _lr;

		private Transform _notActualOrigin;
		private Transform _origin;
		private Rigidbody _rb;
		private Vector3[] _ropePositions;
		private Vector3[] _ropeVelocities;

		private void Awake() {
			_rb = GetComponent<Rigidbody>();
			_lr = GetComponent<LineRenderer>();

			_lr.startWidth = ropeWidth;
			_lr.endWidth = ropeWidth;
			_lr.material = ropeMaterial;

			_ropePositions = new Vector3[ropeSegments];
			_ropeVelocities = new Vector3[ropeSegments];
		}

		private void Start() {
			if (_notActualOrigin != null)
				for (var i = 0; i < ropeSegments; i++) {
					_ropePositions[i] = _notActualOrigin.position;
					_ropeVelocities[i] = Vector3.zero;
				}
			_attachmentPoint = transform.position;
		}

		private void Update() {
			if (!hit) {
				if (_notActualOrigin) {
					_lr.positionCount = 2;
					_lr.SetPosition(0, _notActualOrigin.position);
					_lr.SetPosition(1, transform.position);
				}
			} else {
				UpdateRopeVisual();
			}

			if (_isPulling && hit) Pull();
			SimulateRope();
		}

		private void OnTriggerEnter(Collider other) {
			if (hit || other == null) return;

			var loot = other.GetComponent<Loot.Loot>();
			if (loot != null) AttachTo(other.transform);
		}

		public void Init(Transform notActualOrigin, Transform origin, float force) {
			_notActualOrigin = notActualOrigin;
			_origin = origin;

			if (_rb != null && _origin != null)
				_rb.AddForce(_origin.forward * force, ForceMode.Impulse);
		}

		private void SimulateRope() {
			if (transform.parent) {
				var targetAttachment = transform.parent.TransformPoint(transform.localPosition);
				_attachmentPoint = Vector3.Lerp(_attachmentPoint, targetAttachment, 0.3f);
			}

			_ropePositions[0] = _notActualOrigin.position;

			for (var i = 1; i < ropeSegments; i++) {
				Vector3 targetPos;
				if (i == ropeSegments - 1) {
					targetPos = _attachmentPoint;
				} else {
					var t = i / (float)(ropeSegments - 1);
					targetPos = Vector3.Lerp(_notActualOrigin.position, _attachmentPoint, t);

					var dir = (_attachmentPoint - _notActualOrigin.position).normalized;
					var perp = Vector3.Cross(dir, Vector3.up).normalized;
					var sag = Mathf.Sin(t * Mathf.PI) * ropeSag;
					targetPos += perp * sag;
				}

				var displacement = _ropePositions[i] - targetPos;
				var springForce = -ropeSpring * displacement;
				_ropeVelocities[i] += springForce * Time.fixedDeltaTime;
				_ropeVelocities[i] *= 1f - ropeDamping * Time.fixedDeltaTime;

				_ropePositions[i] += _ropeVelocities[i] * Time.fixedDeltaTime;
			}
		}

		private void UpdateRopeVisual() {
			_lr.positionCount = ropeSegments;
			_lr.SetPositions(_ropePositions);
		}

		private void AttachTo(Transform target) {
			hit = true;
			if (_rb != null) {
				_rb.constraints = RigidbodyConstraints.FreezeAll;
				_rb.linearVelocity = Vector3.zero;
				_rb.angularVelocity = Vector3.zero;
			}
			transform.SetParent(target);
			transform.localPosition = Vector3.zero;

			if (_notActualOrigin != null)
				for (var i = 0; i < ropeSegments; i++) {
					var t = i / (float)(ropeSegments - 1);
					_ropePositions[i] =
						Vector3.Lerp(_notActualOrigin.position, transform.position, t);
					_ropeVelocities[i] = Vector3.zero;
				}
		}


		public void StartPull() {
			_isPulling = true;
		}

		public void StopPull() {
			_isPulling = false;
		}

		private void Pull() {
			if (transform.parent != null && _origin != null)
				transform.parent.position = Vector3.MoveTowards(
					transform.parent.position,
					_origin.position,
					pullSpeed * Time.deltaTime
				);
		}
	}
}