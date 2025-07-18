using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
    public class Jetpack : MonoBehaviour
    {
        public float jetpackForce = 10f;

        public float maxFuel = 5f;
        public float fuelBurnRate = 1f;
        public float fuelRestoreRate = 1f;
        public Slider fuelSlider;
        private float _currentFuel;

        private Rigidbody _playerRb;
        private InputSystem_Actions _inputActions;
        private bool _isFlying = false;

        public void Init(Rigidbody playerRb)
        {
            _playerRb = playerRb;
        }

        void Awake()
        {
            _playerRb = GetComponent<Rigidbody>();
            _inputActions = new InputSystem_Actions();
            _currentFuel = maxFuel; // ← Initialize fuel
        }

        void Start()
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = _currentFuel;
        }

        void OnEnable()
        {
            _inputActions.Jetpack.Enable();
            _inputActions.Jetpack.Use.performed += ToggleJetpack;
            _inputActions.Jetpack.Use.canceled += ToggleJetpack;
        }

        void OnDisable()
        {
            _inputActions.Jetpack.Use.performed -= ToggleJetpack;
            _inputActions.Jetpack.Use.canceled -= ToggleJetpack;
            _inputActions.Jetpack.Disable();
        }

        void ToggleJetpack(InputAction.CallbackContext _)
        {
            if (_currentFuel > 0f) // ← Only allow if there's fuel
                _isFlying = !_isFlying;
        }

        void FixedUpdate()
        {
            if (!_isFlying)
            {
                _currentFuel += fuelRestoreRate * Time.fixedDeltaTime;
                fuelSlider.value = _currentFuel;
                return;
            }

            _playerRb.AddForce(_playerRb.transform.up * jetpackForce, ForceMode.Acceleration);
            _currentFuel -= fuelBurnRate * Time.fixedDeltaTime;

            if (_currentFuel <= 0f)
            {
                _currentFuel = 0f;
                _isFlying = false;
            }

            fuelSlider.value = _currentFuel;
        }
    }
}