using System;
using System.Collections;
using Notifications;
using Player;
using Projectile;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy {
	public class SharkAi : MonoBehaviour {
		public GameObject bloodParticlePrefab;
		public int shotsRequiredToBeScared = 3;
		public int attackDmg;
		public int maxAttackPerTime = 5;
		[Header("Speed")] public float normalSpeed = 20f;
		public float aggressiveSpeed = 20f;
		[Header("Distance")] public float patrolOff = 25f;
		public float voidOff = 400f;
		[Header("Timing")] public float patrolDuration = 20f;
		public float attackCooldown = 2f;
		private int _attackCounter;
		private float _attackTimer;
		private NotificationManager _notificationManager;
		private Vector3 _orbit;
		private Vector3 _patrolDist;
		private float _patrolTimer;
		private bool _pollingTarget;
		private int _shotsGotCounter;
		private State _state;
		private Transform _target;
		private PlayerNeeds _targetNeeds;
		private Vector3 _void;

		private void Start() {
			StartCoroutine(PollTarget());
			_patrolTimer = patrolDuration;
			_notificationManager = FindAnyObjectByType<NotificationManager>();
		}

		private void FixedUpdate() {
			if (ReferenceEquals(_target, null) || _target.IsDestroyed()) {
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

		private void OnTriggerEnter(Collider other) {
			if (!other.TryGetComponent(out Harpoon harpoon)) return;
			TakeShot(harpoon.transform.position);
		}

		public void TakeShot(Vector3 pos) {
			Instantiate(bloodParticlePrefab, pos, Quaternion.identity);
			_shotsGotCounter += 1;
			if (_shotsGotCounter == shotsRequiredToBeScared) {
				_void = (_orbit - _target.transform.position).normalized * voidOff;
				_state = State.MovingToVoid;
				_target.GetComponent<PlayerController>().attackedByShark = false;
			}
		}

		private IEnumerator PollTarget() {
			_pollingTarget = true;

			while (true) {
				_targetNeeds = FindAnyObjectByType<PlayerNeeds>();
				if (ReferenceEquals(_targetNeeds, null)) {
					yield return new WaitForSeconds(2f);
					continue;
				}
				break;
			}

			_target = _targetNeeds.GetComponentInChildren<Rigidbody>().transform;

			var dist = (transform.position - _target.transform.position).normalized;
			_patrolDist = dist * patrolOff;

			_orbit = _target.transform.position + _patrolDist;
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
					_notificationManager.ShowNotification(
						"Dinner time for the shark... and you're the menu!");
				}
				_state = State.MovingToTarget;
			}
		}

		private void MoveToTarget() {
			transform.position = Vector3.MoveTowards(
				transform.position,
				_target.transform.position,
				aggressiveSpeed * Time.fixedDeltaTime
			);

			LookAtTarget();

			if (Vector3.Distance(transform.position, _target.transform.position) < 1f) {
				_orbit = _target.transform.position + _patrolDist.normalized * patrolOff;
				_state = State.AttackingTarget;
				_attackTimer = attackCooldown;
				_target.GetComponent<PlayerController>().attackedByShark = true;
			}
		}

		private void AttackTarget() {
			_attackTimer -= Time.fixedDeltaTime;

			if (_attackTimer > 0f) return;

			_targetNeeds.health -= attackDmg;
			_attackCounter += 1;

			if (_attackCounter == maxAttackPerTime) {
				_void = (_orbit - _target.transform.position).normalized * voidOff;
				_state = State.MovingToVoid;
				_target.GetComponent<PlayerController>().attackedByShark = false;
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