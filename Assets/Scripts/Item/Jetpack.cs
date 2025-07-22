using System.Collections.Generic;
using Notifications;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Item {
    public class Jetpack : MonoBehaviour, IUsableItem {
        public Slider fuelSlider;
        public float forceForward = 10f;
        public float forceUp = 10f;
        public float maxFuel = 5f;

        private float _currentFuel;
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
            fuelSlider.maxValue = maxFuel;
            _notificationManager ??= FindObjectOfType<NotificationManager>();
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

        private void FixedUpdate() {
            if (_currentFuel <= 0f) {
                _burningForward = false;
                _burningUp = false;
                return;
            }

            if (_burningForward) Burn(_cameraTransform.forward * forceForward);
            if (_burningUp) Burn(_cameraTransform.up * forceUp);
        }

        private void Burn(Vector3 force) {
            _playerRb.AddForce(force, ForceMode.Acceleration);
            _currentFuel = Mathf.Max(0f, _currentFuel - Time.fixedDeltaTime);
            fuelSlider.value = _currentFuel;
        }

        private void OnToggleBurnForward(InputAction.CallbackContext _) {
            if (_currentFuel > 0f)
                _burningForward = !_burningForward;
        }

        private void OnToggleBurnUp(InputAction.CallbackContext _) {
            if (_currentFuel > 0f)
                _burningUp = !_burningUp;
        }

        // public void Init(Rigidbody playerRb, Transform cameraTransform) {
        //     _playerRb = playerRb;
        //     _cameraTransform = cameraTransform;
        //     _notificationManager ??= FindObjectOfType<NotificationManager>();
        //
        //     _currentFuel = maxFuel;
        //     fuelSlider.value = _currentFuel;
        //
        //     _notificationManager?.ShowNotification("Left click to fly forward, Right click to fly up!");
        // }


        public void Init(Transform origin, List<ItemParameter> itemParameters)
        {
            _playerRb = origin.GetComponent<Rigidbody>();
            _cameraTransform = Camera.main.transform; // fallback
            _notificationManager ??= FindObjectOfType<NotificationManager>();
         
            _currentFuel = maxFuel;
            fuelSlider.value = _currentFuel;
         
            _notificationManager?.ShowNotification("Jetpack initialized with Transform origin.");
        }

        public void Use() {
            _notificationManager?.ShowNotification("Jetpack activated.");
            _currentFuel = maxFuel;
            fuelSlider.value = _currentFuel;
        }

        public void Unuse() {
            _notificationManager?.ShowNotification("Jetpack deactivated.");
            _burningForward = false;
            _burningUp = false;
        }
    }
}
