using System.Collections.Generic;
using UnityEngine;

namespace Projectile {
	public class Harpoon : MonoBehaviour {
		public float trailDuration = 0.1f;
		private readonly List<Vector3> _positions = new();
		private Vector3 _dir;
		private float _force;
		private LineRenderer _lr;
		private Rigidbody _rb;
		private float _timeSinceStart;

		private void Start() {
			_rb = GetComponent<Rigidbody>();
			_lr = GetComponent<LineRenderer>();

			_rb.AddForce(_dir * _force);
			_positions.Add(transform.position);
		}

		private void FixedUpdate() {
			_timeSinceStart += Time.fixedDeltaTime;
			_positions.Add(transform.position);

			if (_timeSinceStart > trailDuration) _positions.RemoveAt(0);

			_lr.positionCount = _positions.Count;
			for (var i = 0; i < _positions.Count; i++) _lr.SetPosition(i, _positions[i]);
		}

		public void Init(Vector3 dir, float force) {
			_dir = dir;
			_force = force;
		}
	}
}