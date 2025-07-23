using Env;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player 
{
    public class PlayerController : NetworkBehaviour 
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float airControl = 0.5f;
        public float rotationSmoothness = 5f;
        public float groundDrag = 5f;
        public float airDrag = 0.5f;

        [Header("Jump Settings")]
        public float jumpForce = 5f;
        public float jumpCooldown = 0.3f;

        [Header("Ground Check")]
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;
        public bool isGrounded;
        
        [Header("Camera Settings")]
        public GameObject CinemachineCameraTarget;
        public float BottomClamp = -90.0f;
        public float TopClamp = 90.0f;
        public float RotationSpeed = 1.0f;

        [Header("References")]
        public Transform ItemHolder;
        public Animator animator;
      //  public GameObject handsObj;

        private Asteroid currentAsteroid;
        private Rigidbody _rb;
        private PlayerInventoryNew _playerInventory;
        private InputSystem_Actions _playerInputActions;
        private bool _isMoving;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private float _cinemachineTargetPitch;
        private float _lastJumpTime;
        private Vector3 _gravityDirection = Vector3.down;
        private float _gravityStrength = 9.81f;
        protected override void OnNetworkPostSpawn() {
            _playerInventory = NetworkManager.Singleton.LocalClient.PlayerObject
                .GetComponent<PlayerInventoryNew>();
        }
        private void Awake() 
        {
            _playerInputActions = new InputSystem_Actions();
            _rb = GetComponent<Rigidbody>();
        }

        private void Start() 
        {
            if (!IsOwner) return;
            
            animator = GetComponent<Animator>();
            _rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            UpdateAsteroidGravity();
            HandleGravity();
            GroundedCheck();
            HandleMovement();
            HandleRotation();
            UpdateAnimations();
        }

        private void UpdateAsteroidGravity()
        {
            currentAsteroid = GetClosestAsteroid();
            
            if (currentAsteroid && currentAsteroid.IsWithinGravityField(transform.position))
            {
                _gravityDirection = currentAsteroid.GetGravityDirection(transform.position);
                _gravityStrength = currentAsteroid.GetGravity(transform.position).magnitude;
                
                AlignToGravity();
            }
            else
            {
                _gravityDirection = Vector3.down;
                _gravityStrength = 9.81f;
            }
        }

        private void AlignToGravity()
        {
            var gravityUp = -_gravityDirection;
            var targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
        }

        private void HandleGravity()
        {
            _rb.AddForce(_gravityDirection * _gravityStrength, ForceMode.Acceleration);
        }

        private void HandleMovement()
        {
            Vector3 moveDirection = transform.forward * _moveInput.y + transform.right * _moveInput.x;
            
            float forceMultiplier = isGrounded ? moveSpeed : moveSpeed * airControl;
            _rb.AddForce(moveDirection.normalized * (forceMultiplier * 10f), ForceMode.Force);

            _rb.linearDamping = isGrounded ? groundDrag : airDrag;
        }

        private void HandleRotation()
        {
            if (_lookInput.sqrMagnitude >= 0.01f)
            {
                _cinemachineTargetPitch += _lookInput.y * RotationSpeed * Time.deltaTime;
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
                
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0f);
                transform.Rotate(Vector3.up * (_lookInput.x * RotationSpeed * Time.deltaTime));
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = transform.position - transform.up * GroundedOffset;
            isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void UpdateAnimations()
        {
            _isMoving = _moveInput.sqrMagnitude > 0.01f;
            animator.SetFloat("moveX", _moveInput.x);
            animator.SetFloat("moveZ", _moveInput.y);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isMoving", _isMoving);
        }

        private Asteroid GetClosestAsteroid()
        {
            Asteroid closest = null;
            float minDist = float.MaxValue;

            foreach (var a in Asteroid.All)
            {
                float dist = Vector3.Distance(transform.position, a.transform.position);
                if (dist < minDist)
                {
                    closest = a;
                    minDist = dist;
                }
            }

            return closest;
        }

        private void Jump()
        {
            if (Time.time - _lastJumpTime < jumpCooldown || !isGrounded) return;
            
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            
            Vector3 jumpDirection = currentAsteroid != null ? 
                -currentAsteroid.GetGravityDirection(transform.position) : 
                Vector3.up;
                
            _rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            _lastJumpTime = Time.time;
        }
    
        private void OnEnable() 
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Move.performed += OnMove;
            _playerInputActions.Player.Move.canceled += OnMove;
            _playerInputActions.Player.Jump.performed += OnJump;
            _playerInputActions.Player.Look.performed += OnLook;
            _playerInputActions.Player.AddAllItems.performed += OnAddAllItems;
        }

        private void OnDisable() 
        {
            _playerInputActions.Player.Jump.performed -= OnJump;
            _playerInputActions.Player.Move.canceled -= OnMove;
            _playerInputActions.Player.Move.performed -= OnMove;
            _playerInputActions.Player.Look.performed -= OnLook;
            _playerInputActions.Player.AddAllItems.performed -= OnAddAllItems;
            _playerInputActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
        private void OnJump(InputAction.CallbackContext context) { if (IsOwner && context.performed && isGrounded) Jump(); }
        private void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

        private void OnAddAllItems(InputAction.CallbackContext context)
        {
            Debug.Log("ASdasd");
            if (!IsOwner) return;
            if (_playerInventory == null) {
                Debug.LogWarning("PlayerInventoryNew not initialized");
                return;
            }
            _playerInventory.AddItemServerRpc("BuildingTool", 1,null);
            _playerInventory.AddItemServerRpc("Jetpack", 1,null);
            _playerInventory.AddItemServerRpc("Edible", 1,null);
            _playerInventory.AddItemServerRpc("GravityGun", 1,null);
            _playerInventory.AddItemServerRpc("HarpoonGun", 1,null);
            _playerInventory.AddItemServerRpc("Jetpack", 1,null);
            _playerInventory.AddItemServerRpc("Metal", 1,null);
            _playerInventory.AddItemServerRpc("Planks", 1,null);
            _playerInventory.AddItemServerRpc("Poison", 1,null);
            _playerInventory.AddItemServerRpc("Potable", 1,null);
            _playerInventory.AddItemServerRpc("Tablet", 1,null);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}