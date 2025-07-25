using System.Collections.Generic;
using Notifications;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Magnet : MonoBehaviour, IUsableItem {
		private static readonly int IsPulling = Animator.StringToHash("isPulling");
		[SerializeField] private float pullSpeed = 25f;
		[SerializeField] private float maxDistance = 1000000f;
		[SerializeField] private float grabDistance = 2f;
		[SerializeField] private List<LineRenderer> beamLines;
		[SerializeField] private AudioSource magnetSound;
		private Animator _anim;
		private InputSystem_Actions _ia;
		private bool _isActive;
		private NotificationManager _notificationManager;
		private Transform _origin;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			_anim = GetComponent<Animator>();
		}

		private void FixedUpdate() {
			if (!_isActive || !_origin) {
				foreach (var beamLine in beamLines) beamLine.enabled = false;
				return;
			}

			var hitSomething = Physics.Raycast(_origin.position, _origin.forward,
				out var laserHit, maxDistance);

			if (hitSomething) {
				if (laserHit.collider.TryGetComponent(out Loot.Loot _currentLoot)) {
					_anim.SetBool(IsPulling, true);
					magnetSound.Play();
				}

				var distance =
					Vector3.Distance(_currentLoot.transform.position, _origin.position);

				if (distance <= grabDistance) {
					foreach (var beamLine in beamLines) beamLine.enabled = false;
					return;
				}

				_currentLoot.transform.position = Vector3.MoveTowards(
					_currentLoot.transform.position,
					_origin.position,
					pullSpeed * Time.fixedDeltaTime
				);
				foreach (var beamLine in beamLines) {
					beamLine.enabled = true;
					beamLine.SetPosition(0, beamLine.transform.position);
					beamLine.SetPosition(1, laserHit.point);
				}
			} else {
				foreach (var beamLine in beamLines) beamLine.enabled = false;
				_anim.SetBool(IsPulling, false);
				magnetSound.Stop();
			}
		}

		public void OnEnable() {
			_ia.Player.Enable();
			_ia.Player.UseItem.started += Use;
			_ia.Player.UseItem.canceled += Unuse;
		}

		public void OnDisable() {
			_ia.Player.UseItem.started -= Use;
			_ia.Player.UseItem.canceled -= Unuse;
			_ia.Player.Disable();
		}

		private void OnDrawGizmosSelected() {
			if (_origin != null) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(_origin.position, _origin.position + _origin.forward * maxDistance);
			}
		}

		public void Init(Transform origin, List<ItemParameter> itemParameters) {
			_origin = origin;

			transform.rotation = _origin.rotation * Quaternion.Euler(-11f, -90f, 96f);
			foreach (var beamLine in beamLines) beamLine.enabled = false;

			_notificationManager = FindFirstObjectByType<NotificationManager>();
		}

		private void Use(InputAction.CallbackContext context) {
			if (_isActive) return;
			_isActive = true;
			_notificationManager?.ShowNotification(
				"Aim at loot and hold the button to pull it toward you!");
		}

		private void Unuse(InputAction.CallbackContext context) {
			if (!_isActive) return;
			_anim.SetBool(IsPulling, false);
			magnetSound.Stop();
			_isActive = false;
			foreach (var beamLine in beamLines) beamLine.enabled = false;
		}
	}
}