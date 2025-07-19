using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

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
		private Rigidbody _playerRb;
		private Slot _slot;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			fuelSlider.maxValue = cap;
			fuelSlider.value = _slot.JetpackFuel ?? cap;
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

		public void Init(Rigidbody playerRb, Transform cameraTransform, Slot slot) {
			_playerRb = playerRb;
			_cameraTransform = cameraTransform;
			_slot = slot;

			_slot.JetpackFuel ??= cap;
		}

		private void OnToggleBurnForward(InputAction.CallbackContext _) {
			Debug.Assert(_slot.JetpackFuel != null);

			if (_slot.JetpackFuel.Value == 0f) return;
			_burningForward = !_burningForward;
		}

		private void OnToggleBurnUp(InputAction.CallbackContext _) {
			Debug.Assert(_slot.JetpackFuel != null);

			if (_slot.JetpackFuel.Value == 0f) return;
			_burningUp = !_burningUp;
		}

		private void Burn(Vector3 force) {
			Debug.Assert(_slot.JetpackFuel != null);

			_playerRb.AddForce(force, ForceMode.Acceleration);
			_slot.JetpackFuel = Mathf.Max(0, _slot.JetpackFuel.Value - Time.fixedDeltaTime);

			if (_slot.JetpackFuel.Value == 0f) {
				_burningForward = false;
				_burningUp = false;
			}

			fuelSlider.value = _slot.JetpackFuel.Value;
		}
	}
}