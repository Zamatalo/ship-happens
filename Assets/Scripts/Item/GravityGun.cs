using System;
using System.Collections.Generic;
using Notifications;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class GravityGun : MonoBehaviour, IUsableItem {
		[SerializeField] private float pullSpeed = 25f;
		[SerializeField] private float maxDistance = 1000000f;
		[SerializeField] private float grabDistance = 2f;
		[SerializeField] private List<LineRenderer> beamLines;
		[SerializeField] private AudioSource magnetSound;
		[SerializeField] private Animator animator;
		private Loot.Loot _currentLoot;
		private bool _isActive;
		private NotificationManager _notificationManager;
		private Transform _origin;

		private InputSystem_Actions _ia;
		private void Awake()
		{
			_ia = new InputSystem_Actions();
		}

		private void Update() {
			if (!_isActive || !_origin) {
				foreach (var beamLine in beamLines) beamLine.enabled = false;
				return;
			}

			var hitSomething = Physics.Raycast(_origin.position, _origin.forward,
				out var laserHit, maxDistance);

			if (_currentLoot) {
				var distance =
					Vector3.Distance(_currentLoot.transform.position, _origin.position);

				if (distance <= grabDistance) {
					_currentLoot = null;
					foreach (var beamLine in beamLines) beamLine.enabled = false;
					return;
				}

				_currentLoot.transform.position = Vector3.MoveTowards(
					_currentLoot.transform.position,
					_origin.position,
					pullSpeed * Time.fixedDeltaTime
				);
			} else if (hitSomething) {
				if (laserHit.collider.TryGetComponent(out _currentLoot)) {
					animator.SetBool("isPulling", true);
					magnetSound.Play();
					Debug.Log($"Pulling loot: {_currentLoot.name}");
				} else return;
			}

			if (_currentLoot || hitSomething)
				for (var i = 0; i < beamLines.Count; i++) {
					beamLines[i].enabled = true;
					beamLines[i].SetPosition(0, beamLines[i].transform.position);
					beamLines[i].SetPosition(1,
						_currentLoot ? _currentLoot.transform.position : laserHit.point);
				}
			else {
				foreach (var beamLine in beamLines)
					beamLine.enabled = false;
			}
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

			foreach (var beamLine in beamLines)
				beamLine.enabled = false;

			_notificationManager = FindFirstObjectByType<NotificationManager>();
		}

		private void Use(InputAction.CallbackContext context) {
			if (_isActive) return;
			_isActive = true;
			_notificationManager?.ShowNotification(
				" Aim at loot and hold the button to pull it toward you!");
		}

		private void Unuse(InputAction.CallbackContext context) {
			if (!_isActive) return;
			animator.SetBool("isPulling", false);
			magnetSound.Stop();
			_isActive = false;
			_currentLoot = null;
			foreach (var beamLine in beamLines) beamLine.enabled = false;
		}

		private void OnDestroy()
		{
			OnDisable();
		}

		public void OnEnable()
		{
			_ia.Player.Enable();
			_ia.Player.UseItem.started += Use;
			_ia.Player.UseItem.canceled += Unuse;
		}

		public void OnDisable()
		{
			_ia.Player.UseItem.started -= Use;
			_ia.Player.UseItem.canceled -= Unuse;
			_ia.Player.Disable();
		}
	}
}