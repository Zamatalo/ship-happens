// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using StarterAssets;
//
// public class PlayerController_Cinemachine : NetworkBehaviour
// {
//     //---------------MOVEMENT---------------
//     public float moveSpeed = 5f;
//     public float RotationSpeed = 1.0f;
//     public float SpeedChangeRate = 10.0f;
//     public float JumpHeight = 1.2f;
//     public float Gravity = -15.0f;
//     public float SprintSpeed = 6.0f;
//     private float terminalVelocity = 53.0f;
//
//     //---------------OBJECTS---------------
//     private CharacterController controller;
//     private StarterAssetsInputs _input;
//     private PlayerInput playerInput;
//
//     //---------------GROUND---------------
//     public bool isGrounded = true;
//     public float GroundedOffset = -0.14f;
//     public float GroundedRadius = 0.5f;
//     public LayerMask GroundLayers;
//
//     //---------------ANIMATOR---------------
//     private bool isMoving;
//     public Animator animator;
//
//     //---------------CAMERA---------------
//     private float cinemachineTargetPitch;
//     public GameObject CinemachineCameraTarget;
//     private GameObject mainCamera;
//     public float BottomClamp = -90.0f;
//     public float TopClamp = 90.0f;
//
//     //---------------UTIL---------------
//     private const float _threshold = 0.01f;
//     private float rotationVelocity;
//     private float verticalVelocity;
//     private float speed;
//
//     private void Awake()
//     {
//         mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
//     }
//
//     private void Start()
//     {
//         animator = GetComponent<Animator>();
//         controller = GetComponent<CharacterController>();
//         playerInput = GetComponent<PlayerInput>();
//         _input = GetComponent<StarterAssetsInputs>();
//     }
//
//     private void Update()
//     {
//         if (!IsOwner) return;
//
//         JumpAndGravity();
//         GroundedCheck();
//         Move();
//
//         isMoving = _input.move.magnitude > 0.01f;
//
//         animator.SetFloat("moveX", _input.move.x);
//         animator.SetFloat("moveZ", _input.move.y);
//         animator.SetBool("isGrounded", isGrounded);
//         animator.SetBool("isMoving", isMoving);
//         animator.SetBool("isSprinting", _input.sprint);
//
//         if (UnityEngine.Input.GetKeyDown(KeyCode.P))
//         {
//             if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<PlayerInventoryNew>(out var inventory))
//             {
//                 Debug.Log("Trying to add to player inventory");
//                 inventory.AddItemServerRpc("Poison", 1);
//                 inventory.AddItemServerRpc("Metal", 1);
//                 inventory.AddItemServerRpc("Harpoon", 1);
//                 inventory.AddItemServerRpc("Magnet", 1);
//             }
//         }
//     }
//
//     private void LateUpdate()
//     {
//         if (!IsOwner) return;
//         CameraRotation();
//     }
//
//     private void GroundedCheck()
//     {
//         Vector3 spherePosition = transform.position + Vector3.down * GroundedOffset;
//         isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
//     }
//
//     private void CameraRotation()
//     {
//         if (CinemachineCameraTarget == null || _input.look.sqrMagnitude < _threshold) return;
//
//         var deltaTime = Time.deltaTime;
//         cinemachineTargetPitch += _input.look.y * RotationSpeed;
//         rotationVelocity = _input.look.x * RotationSpeed;
//
//         cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
//         CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
//         transform.Rotate(Vector3.up * rotationVelocity * deltaTime);
//     }
//
//     private static float ClampAngle(float angle, float min, float max)
//     {
//         if (angle < -360f) angle += 360f;
//         if (angle > 360f) angle -= 360f;
//         return Mathf.Clamp(angle, min, max);
//     }
//
//     private void Move()
//     {
//         float targetSpeed = _input.sprint ? SprintSpeed : moveSpeed;
//         if (_input.move == Vector2.zero) targetSpeed = 0.0f;
//
//         float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
//         float speedOffset = 0.1f;
//         float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
//
//         if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
//         {
//             speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
//             speed = Mathf.Round(speed * 1000f) / 1000f;
//         }
//         else
//         {
//             speed = targetSpeed;
//         }
//
//         Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
//         if (_input.move != Vector2.zero)
//         {
//             inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
//         }
//
//         if (controller.enabled)
//         {
//             controller.Move(inputDirection.normalized * (speed * Time.deltaTime) + Vector3.up * verticalVelocity * Time.deltaTime);
//         }
//     }
//
//     private void JumpAndGravity()
//     {
//         if (isGrounded)
//         {
//             if (verticalVelocity < 0.0f)
//                 verticalVelocity = -2f;
//
//             if (_input.jump)
//             {
//                 verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
//                 animator.SetTrigger("Jump");
//                 _input.jump = false;
//             }
//         }
//         else
//         {
//             _input.jump = false;
//         }
//
//         verticalVelocity = Mathf.Max(verticalVelocity + Gravity * Time.deltaTime, terminalVelocity);
//     }
// }
