using Notifications;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Item {
	public class Jetpack : MonoBehaviour {
		public Slider fuelSlider;
		public float forceForward = 10f;
		public float forceUp = 10f;
		public float cap = 5f;
		private bool _burningForward;
		private bool _burningUp;
		private Transform _cameraTransform;
		private InputSystem_Actions _ia;
		private NotificationManager _notificationManager;
		private Rigidbody _playerRb;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			fuelSlider.maxValue = cap;
//			fuelSlider.value =
			//!Mathf.Approximately(_slot.jetpackFuel, -1) ? _slot.jetpackFuel : cap;
			_notificationManager ??= FindObjectOfType<NotificationManager>();
		}

		private void FixedUpdate() {
			if (_burningForward) Burn(_cameraTransform.forward * forceForward);
			if (_burningUp) Burn(_cameraTransform.up * forceUp);
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Jetpack.BurnForward.performed += OnToggleBurnForward;
			_ia.Jetpack.BurnForward.canceled += OnToggleBurnForward;
			_ia.Jetpack.BurnUp.performed += OnToggleBurnUp;
			_ia.Jetpack.BurnUp.canceled += OnToggleBurnUp;
		}

		private void OnDisable() {
			_ia.Jetpack.BurnForward.performed -= OnToggleBurnForward;
			_ia.Jetpack.BurnForward.canceled -= OnToggleBurnForward;
			_ia.Jetpack.BurnUp.performed -= OnToggleBurnUp;
			_ia.Jetpack.BurnUp.canceled -= OnToggleBurnUp;
			_ia.Disable();
		}

		public void Init(Rigidbody playerRb, Transform cameraTransform) {
			_playerRb = playerRb;
			_cameraTransform = cameraTransform;

			//_slot.jetpackFuel =
			//	!Mathf.Approximately(_slot.jetpackFuel, -1) ? _slot.jetpackFuel : cap;
			_notificationManager ??= FindObjectOfType<NotificationManager>();
			_notificationManager?.ShowNotification(
				"Left click to fly forward, Right click to fly up!");
		}

		private void OnToggleBurnForward(InputAction.CallbackContext _) {
			//	if (_slot.jetpackFuel == 0f) return;
			_burningForward = !_burningForward;
		}

		private void OnToggleBurnUp(InputAction.CallbackContext _) {
//			if (_slot.jetpackFuel == 0f) return;
			_burningUp = !_burningUp;
		}

		private void Burn(Vector3 force) {
			_playerRb.AddForce(force, ForceMode.Acceleration);
			//_slot.jetpackFuel = Mathf.Max(0, _slot.jetpackFuel - Time.fixedDeltaTime);

			//if (_slot.jetpackFuel == 0f) {
			_burningForward = false;
			_burningUp = false;
			//}

			//fuelSlider.value = _slot.jetpackFuel;
		}
	}
}