using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Jetpack : MonoBehaviour
    {
        public float jetpackForce = 10f;
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
            _isFlying = !_isFlying;
        }

        void FixedUpdate()
        {
            if (_isFlying)
            {
                _playerRb.AddForce(Vector3.up * jetpackForce, ForceMode.Acceleration);
            }
        }
    }
}

