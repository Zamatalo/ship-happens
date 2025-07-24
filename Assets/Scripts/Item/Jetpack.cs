using System.Collections.Generic;
using Notifications;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Item {
	[RequireComponent(typeof(AudioSource))]
	public class Jetpack : MonoBehaviour, IUsableItem {
		public AudioClip burnSound;
		public Slider fuelSlider;
		public float forceForward = 1000f;
		public float forceUp = 20f;
		public float maxFuel = 10f;
		public float maxMag = 20f;
		private AudioSource _audio;
		private bool _burningForward;
		private bool _burningUp;
		private Transform _cameraTransform;
		private float _currentFuel;
		private InputSystem_Actions _ia;
		private NotificationManager _notificationManager;
		private Rigidbody _playerRb;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			_audio = GetComponent<AudioSource>();
		}

		private void FixedUpdate() {
			if (_currentFuel <= 0f) {
				_burningForward = false;
				_burningUp = false;
				return;
			}

			//if (!_burningForward && !_burningUp) Restore();
			if (_burningForward) Burn(_cameraTransform.forward * forceForward);
			if (_burningUp) Burn(_cameraTransform.up * forceUp);

			ClampMag();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Jetpack.BurnForward.performed += StartBurnForward;
			_ia.Jetpack.BurnForward.canceled += EndBurnForward;
			_ia.Jetpack.BurnUp.performed += StartBurnUp;
			_ia.Jetpack.BurnUp.canceled += EndBurnUp;
		}

		private void OnDisable() {
			_ia.Jetpack.BurnForward.performed -= StartBurnForward;
			_ia.Jetpack.BurnForward.canceled -= EndBurnForward;
			_ia.Jetpack.BurnUp.performed -= StartBurnUp;
			_ia.Jetpack.BurnUp.canceled -= EndBurnUp;
			_ia.Disable();
		}

		public void Init(Transform origin, List<ItemParameter> itemParameters) {
			_playerRb = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponentInChildren<Rigidbody>();
			_cameraTransform = origin;
			_notificationManager = FindAnyObjectByType<NotificationManager>();

			_currentFuel = maxFuel;
			fuelSlider.maxValue = maxFuel;
			fuelSlider.value = _currentFuel;
		}


		private void Burn(Vector3 force) {
			_playerRb.AddForce(force, ForceMode.Acceleration);
			_currentFuel = Mathf.Max(0f, _currentFuel - Time.fixedDeltaTime);
			fuelSlider.value = _currentFuel;
		}

		private void Restore() {
			_currentFuel = Mathf.Min(maxFuel, _currentFuel + Time.fixedDeltaTime);
			fuelSlider.value = _currentFuel;
		}

		private void StartBurnForward(InputAction.CallbackContext _) {
			if (!_burningForward && !_burningUp) _audio.PlayOneShot(burnSound);
			if (_currentFuel > 0f)
				_burningForward = true;
		}

		private void EndBurnForward(InputAction.CallbackContext _) {
			if (_burningForward && !_burningUp) _audio.Stop();
			_burningForward = false;
		}

		private void StartBurnUp(InputAction.CallbackContext _) {
			if (!_burningForward && !_burningUp) _audio.PlayOneShot(burnSound);
			if (_currentFuel > 0f)
				_burningUp = true;
		}

		private void EndBurnUp(InputAction.CallbackContext _) {
			if (!_burningForward && _burningUp) _audio.Stop();
			_burningUp = false;
		}

		private void ClampMag() {
			var vel = _playerRb.linearVelocity;
			var mag = vel.magnitude;

			if (mag > maxMag) _playerRb.linearVelocity = vel * (maxMag / mag);
		}
	}
}