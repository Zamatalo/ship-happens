using System;
using System.Collections;
using BuildKit;
using UnityEngine;

namespace Enemy {
	public class SharkAi : MonoBehaviour {
		[Header("Speed")] public float normalSpeed = 20f;
		public float aggressiveSpeed = 20f;
		[Header("Distance")] public float patrolOff = 25f;
		public float voidOff = 400f;
		[Header("Timing")] public float patrolDuration = 20f;
		public float attackCooldown = 2f;
		private float _attackTimer;
		private Vector3 _orbit;
		private Vector3 _patrolDist;
		private float _patrolTimer;
		private bool _pollingTarget;
		private State _state = State.MovingToOrbit;
		private Building _target;
		private Vector3 _void;
		private Notifications.NotificationManager _notificationManager;


		private void Start() {
			StartCoroutine(PollTarget());
			_patrolTimer = patrolDuration;
			_notificationManager = FindObjectOfType<Notifications.NotificationManager>();

		}

		private void FixedUpdate() {
			if (ReferenceEquals(_target, null)) {
				if (!_pollingTarget) StartCoroutine(PollTarget());
				return;
			}

			switch (_state) {
				case State.MovingToOrbit:
					MoveToOrbit();
					break;
				case State.Patrolling:
					Patrol();
					break;
				case State.MovingToTarget:
					MoveToTarget();
					break;
				case State.AttackingTarget:
					AttackTarget();
					break;
				case State.MovingToVoid:
					MoveToVoid();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IEnumerator PollTarget() {
			_pollingTarget = true;

			while (true) {
				_target = FindAnyObjectByType<Building>();
				if (ReferenceEquals(_target, null)) {
					yield return new WaitForSeconds(2f);
					continue;
				}
				break;
			}

			var dist = (transform.position - _target.transform.position).normalized;
			_patrolDist = dist * patrolOff;
			_pollingTarget = false;
		}

		private void MoveToOrbit() {
			var dir = (_orbit - transform.position).normalized;
			transform.position += normalSpeed * Time.fixedDeltaTime * dir;

			LookAtTarget();

			if (Vector3.Distance(transform.position, _orbit) < 1f) {
				_patrolTimer = patrolDuration;
				_state = State.Patrolling;
			}
		}

		private void Patrol() {
			_patrolTimer -= Time.fixedDeltaTime;
			_patrolDist = Quaternion.Euler(0, normalSpeed * Time.fixedDeltaTime, 0) * _patrolDist;
			transform.position = _target.transform.position + _patrolDist;

			LookAtTarget();

			if (_patrolTimer <= 0f) {
				if (_notificationManager != null) {
					_notificationManager.ShowNotification(" Dinner time for the shark... and you're the menu!", 5f);
				}
				_state = State.MovingToTarget;
			}

		}

		private void MoveToTarget() {
			var dir = (_target.transform.position - transform.position).normalized;
			transform.position += aggressiveSpeed * Time.fixedDeltaTime * dir;

			LookAtTarget();

			if (Vector3.Distance(transform.position, _target.transform.position) < 1f) {
				_orbit = _target.transform.position + _patrolDist.normalized * patrolOff;
				_state = State.AttackingTarget;
				_attackTimer = attackCooldown;
			}
		}

		private void AttackTarget() {
			_attackTimer -= Time.fixedDeltaTime;

			if (_attackTimer > 0f) return;

			if (_target.TakeDamage(1)) {
				_void = (_orbit - _target.transform.position).normalized * voidOff;
				_state = State.MovingToVoid;
			}
			_attackTimer = attackCooldown;
		}

		private void MoveToVoid() {
			var dir = (_void - transform.position).normalized;
			transform.position += normalSpeed * Time.fixedDeltaTime * dir;

			LookAtVoid();

			if (Vector3.Distance(transform.position, _void) < 1f) Destroy(gameObject);
		}

		private void LookAtTarget() {
			var dir = (_target.transform.position - transform.position).normalized;
			var rot = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5 * Time.fixedDeltaTime);
		}

		private void LookAtVoid() {
			var dir = (_void - transform.position).normalized;
			var rot = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5 * Time.fixedDeltaTime);
		}

		private enum State {
			MovingToOrbit,
			Patrolling,
			MovingToTarget,
			AttackingTarget,
			MovingToVoid
		}
	}
}