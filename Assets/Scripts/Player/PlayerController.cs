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
        public Animator animator;
        
        
        public bool attackedByShark;
        private Asteroid _currentAsteroid;
        private Rigidbody _rb;
        private bool _isMoving;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private float _cinemachineTargetPitch;
        private float _lastJumpTime;
        private Vector3 _gravityDirection = Vector3.down;
        private float _gravityStrength = 9.81f;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            _rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            var spawnObj = GameObject.FindGameObjectWithTag("SpawnBlock");
            if (spawnObj != null)
            {
                transform.position = spawnObj.transform.position;
            }
            else
            {
                Debug.LogWarning("SpawnBlock not found! Spawning at default position.");
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            UpdateAsteroidGravity();
            HandleGravity();
            GroundedCheck();
            
            if (!attackedByShark)
            {
                HandleMovement();
                HandleRotation();
            }
            
            UpdateAnimations();
        }

        private void UpdateAsteroidGravity()
        {
            _currentAsteroid = GetClosestAsteroid();
            
            if (_currentAsteroid && _currentAsteroid.IsWithinGravityField(transform.position))
            {
                _gravityDirection = _currentAsteroid.GetGravityDirection(transform.position);
                _gravityStrength = _currentAsteroid.GetGravity(transform.position).magnitude;
                
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
            if (attackedByShark) return;
            if (Time.time - _lastJumpTime < jumpCooldown || !isGrounded) return;
            
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            
            Vector3 jumpDirection = _currentAsteroid != null ? 
                -_currentAsteroid.GetGravityDirection(transform.position) : 
                Vector3.up;
                
            _rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            _lastJumpTime = Time.time;
        }

        private void OnMove(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
        private void OnJump(InputAction.CallbackContext context) { if (IsOwner && context.performed && isGrounded) Jump(); }
        private void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}