// using KinematicCharacterController;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// [RequireComponent(typeof(KinematicCharacterMotor))]
// public class Controller : MonoBehaviour, ICharacterController
// {
//     [Header("Camera Settings")] public GameObject cinemachineCameraTarget;
//     public float topClamp = 90f;
//     public float bottomClamp = -90f;
//     public float mouseSensitivity = 1f;
//
//     [Header("Movement")] public float MaxStableMoveSpeed = 10f, StableMovementSharpness = 15f;
//     private KinematicCharacterMotor _motor;
//     public float Gravity = 30f;
//     public float AirAcceleration = 5f;
//     
//     [Header("Jumping")]
//     public float JumpUpSpeed = 10f;
//     public float JumpScalableForwardSpeed = 10f;
//     private bool _jumpRequested = false;
//     private bool _jumpConsumed = false;
//     private float _timeSinceLastAbleToJump = 0f;
//     private bool _jumpedThisFrame = false;
//
//     private Vector2 _moveInput;
//     private Vector2 _lookInput;
//     private float _cinemachineTargetPitch;
//     private Vector3 _moveInputVector;
//     private Quaternion _targetRotation;
//     private Vector3 _gravityVelocity;
//     [Header("Air Movement")] public float AirControl = 0.5f, MaxAirMoveSpeed = 15f;
//
//
//     private void Awake()
//     {
//         _motor = GetComponent<KinematicCharacterMotor>();
//         _motor.CharacterController = this;
//
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;
//     }
//
//     public void OnMove(InputAction.CallbackContext context)
//     {
//         _moveInput = context.ReadValue<Vector2>();
//         _moveInputVector = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
//     }
//
//     public void OnLook(InputAction.CallbackContext context)
//     {
//         _lookInput = context.ReadValue<Vector2>();
//     }
//
//     public void OnJump(InputAction.CallbackContext context)
//     {
//         _jumpRequested = true;
//     }
//
//     private void LateUpdate()
//     {
//         HandleCameraRotation();
//     }
//
//     private void HandleCameraRotation()
//     {
//         if (!cinemachineCameraTarget || _lookInput.sqrMagnitude < 0.01f) return;
//
//         float deltaTimeMultiplier = mouseSensitivity * Time.deltaTime;
//
//         _targetRotation = Quaternion.Euler(0f,
//             _motor.TransientRotation.eulerAngles.y + _lookInput.x * deltaTimeMultiplier, 0f);
//         _motor.SetRotation(_targetRotation);
//
//         _cinemachineTargetPitch += _lookInput.y * deltaTimeMultiplier;
//         _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
//
//         cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0f);
//     }
//
//
//
//
//     public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
//     {
//         currentRotation = _targetRotation;
//     }
//
//     public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
//     {
//         if (!_motor.GroundingStatus.IsStableOnGround)
//         {
//             _gravityVelocity += Vector3.down * (Gravity * deltaTime);
//             
//         }
//         else
//         {
//             if (_jumpRequested)
//             {
//                 Vector3 jumpDirection = _motor.CharacterUp;
//                 if (_motor.GroundingStatus.FoundAnyGround && !_motor.GroundingStatus.IsStableOnGround)
//                 {
//                     jumpDirection = _motor.GroundingStatus.GroundNormal;
//                 }
//
//                 // 1. Сначала обрабатываем вертикальную составляющую прыжка
//                 float verticalVelocity = Vector3.Dot(currentVelocity, _motor.CharacterUp);
//                 if (verticalVelocity < JumpUpSpeed)
//                 {
//                     currentVelocity += jumpDirection * (JumpUpSpeed - verticalVelocity);
//                 }
//
//                 // 2. Затем добавляем горизонтальный импульс ПРАВИЛЬНО
//                 if (_moveInputVector.magnitude > 0.1f)
//                 {
//                     // Преобразуем входные данные в мировые координаты
//                     Vector3 worldSpaceMoveInput = _targetRotation * _moveInputVector;
//                     // Проецируем на горизонтальную плоскость
//                     worldSpaceMoveInput = Vector3.ProjectOnPlane(worldSpaceMoveInput, _motor.CharacterUp).normalized;
//                     // Применяем горизонтальную силу
//                     currentVelocity += worldSpaceMoveInput * JumpScalableForwardSpeed;
//                 }
//
//                 _jumpRequested = false;
//                 _jumpConsumed = true;
//                 _jumpedThisFrame = true;
//             }  
//         }
//        
//         var worldspaceMoveInput = _targetRotation * _moveInputVector;
//
//         if (_motor.GroundingStatus.IsStableOnGround)
//         {
//             // Ground movement with slope handling
//             float currentVelocityMagnitude = currentVelocity.magnitude;
//             Vector3 effectiveGroundNormal = _motor.GroundingStatus.GroundNormal;
//
//             // Reorient velocity on slope
//             currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
//                               currentVelocityMagnitude;
//
//             // Calculate target velocity relative to slope
//             Vector3 inputRight = Vector3.Cross(worldspaceMoveInput, _motor.CharacterUp);
//             Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
//                                       worldspaceMoveInput.magnitude;
//             Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;
//
//             currentVelocity = Vector3.Lerp(
//                 currentVelocity,
//                 targetMovementVelocity,
//                 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
//         }
//         else
//         {
//             Vector3 targetVelocity = worldspaceMoveInput * (MaxAirMoveSpeed * AirControl);
//             currentVelocity = Vector3.Lerp(
//                 currentVelocity,
//                 targetVelocity,
//                 1f - Mathf.Exp(-AirAcceleration * deltaTime));
//         }
//
//         currentVelocity += _gravityVelocity;
//     }
//
//
//     private static float ClampAngle(float angle, float min, float max)
//     {
//         if (angle < -360f) angle += 360f;
//         if (angle > 360f) angle -= 360f;
//         return Mathf.Clamp(angle, min, max);
//     }
//
//     public void BeforeCharacterUpdate(float deltaTime) { }
//     public void PostGroundingUpdate(float deltaTime) { }
//     public void AfterCharacterUpdate(float deltaTime) { }
//     public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
//     public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
//     public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
//         Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
//     public bool IsColliderValidForCollisions(Collider coll) => true;
//     public void OnDiscreteCollisionDetected(Collider hitCollider) { }
// }