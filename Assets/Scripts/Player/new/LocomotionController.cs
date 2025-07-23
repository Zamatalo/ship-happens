// using KinematicCharacterController;
// using UnityEngine;
//
// public enum OrientationMethod
//     {
//         TowardsCamera,
//         TowardsMovement,
//     }
//
// public enum JumpStatus
// {
//     None,
//     Requested,
//     Executed,
//     DoubleJumpUsed
// }
// public struct PlayerCharacterInputs
// {
//     public float moveAxisForward;
//     public float moveAxisRight;
//     public Quaternion cameraRotation;
//     public bool JumpDown;
//     public bool JumpHeld;
// }
// public class LocomotionController : MonoBehaviour, ICharacterController
// {
//     public KinematicCharacterMotor Motor;
//     
//     [Header("Stable Movement")]
//     [SerializeField] private float MaxStableMoveSpeed = 10f;
//     [SerializeField] private float StableMovementSharpness = 15f;
//     [SerializeField]private float OrientationSharpness = 10f;
//     [SerializeField] private OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;
//
//     [Header("Air Movement")]
//     [SerializeField] private float MaxAirMoveSpeed = 15f;
//     [SerializeField] private float AirAccelerationSpeed = 15f;
//     [SerializeField]private float Drag = 0.1f;
//     [SerializeField] private Vector3 Gravity = new Vector3(0, -30f, 0);
//     
//     [Header("Jumping")]
//     [SerializeField] private bool AllowJumpingWhenSliding = false;
//     [SerializeField] private bool AllowDoubleJump = false;
//     [SerializeField] private bool AllowWallJump = false;
//     [SerializeField] private float JumpSpeed = 10f;
//     [SerializeField] private float JumpScalableForwardSpeed = 10f;
//     [SerializeField] private float JumpPreGroundingGraceTime = 0f;
//     [SerializeField] private float HighJumpMultiplier =1.5f;
//     [SerializeField] private float LongJumpMultiplier = 1.5f;
//
//
//     
//     public float m_targetForwardAxis,m_targetRightAxis;
//     private Vector3 m_moveInputVector,m_lookInputVector;
//     private int m_latchedJumpType = -1;
//     //jumping
//     private float m_jumpTypeTimer = 0f;
//     private const float JumpTypeDuration = 0.2f;
//     private JumpStatus m_jumpStatus= JumpStatus.None;
//     private float m_timeSinceJumpRequested = Mathf.Infinity;
//     private float m_timeSinceLastAbleToJump = 0f;
//     private Vector3 m_wallJumpNormal;
//     private bool m_hasJumped = false;
//     private bool m_hasDoubleJumped = false;
//     private bool m_hasWallJumped = false;
//     private bool m_canWallJump = false;
//
//     private void Start()
//     {
//         Motor.CharacterController = this;
//     }
//
//     public void SetInputs(ref PlayerCharacterInputs inputs)
//     {   
//         //need to test this
//         m_moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward), 1f);
//
//         var cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
//         if (cameraPlanarDirection.sqrMagnitude == 0f)
//         {
//             cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, Motor.CharacterUp).normalized;
//         }
//         var cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);
//
//         m_targetForwardAxis = inputs.moveAxisForward;
//         m_targetRightAxis = inputs.moveAxisRight;
//         m_moveInputVector = cameraPlanarRotation * m_moveInputVector;
//         switch (OrientationMethod)
//         {
//             case OrientationMethod.TowardsCamera:
//                 m_lookInputVector = cameraPlanarDirection;
//                 break;
//             case OrientationMethod.TowardsMovement:
//                 m_lookInputVector = m_moveInputVector.normalized;
//                 break;
//         }
//
//         if (inputs.JumpDown)
//         {
//             bool canJump = Motor.GroundingStatus.IsStableOnGround || (AllowJumpingWhenSliding&& Motor.GroundingStatus.FoundAnyGround);
//             bool doubleJumpIsAvailable = AllowDoubleJump && !m_hasDoubleJumped;
//             bool wallJumpIsAvailable = AllowWallJump && !m_hasWallJumped;
//             if (canJump)
//             {
//                 m_jumpStatus = JumpStatus.Requested;
//             }
//             else if (doubleJumpIsAvailable) m_jumpStatus = JumpStatus.Requested;
//             else if (wallJumpIsAvailable) m_jumpStatus = JumpStatus.Requested;
//         }
//     }
//     public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
//     {
//         if (!(m_lookInputVector.sqrMagnitude > 0f) || !(OrientationSharpness > 0f)) return;
//         var smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, m_lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
//
//         currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
//     }
//
//     public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
//     {
//         if (Motor.GroundingStatus.IsStableOnGround)
//         {
//             float currentVelocityMagnitude = currentVelocity.magnitude;
//
//             Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
//
//             currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
//                               currentVelocityMagnitude;
//
//             Vector3 inputRight = Vector3.Cross(m_moveInputVector, Motor.CharacterUp);
//             Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
//                                       m_moveInputVector.magnitude;
//             Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;
//
//             currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
//                 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
//         }
//         // Air movement
//         else
//         {
//             if (m_moveInputVector.sqrMagnitude > 0f)
//             {
//                 Vector3 addedVelocity = m_moveInputVector * (AirAccelerationSpeed * deltaTime);
//
//                 Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);
//
//                 if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
//                 {
//                     Vector3 newTotal =
//                         Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
//                     addedVelocity = newTotal - currentVelocityOnInputsPlane;
//                 }
//                 else
//                 {
//                     if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
//                     {
//                         addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
//                     }
//                 }
//
//                 if (Motor.GroundingStatus.FoundAnyGround)
//                 {
//                     if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
//                     {
//                         Vector3 perpenticularObstructionNormal = Vector3
//                             .Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal),
//                                 Motor.CharacterUp).normalized;
//                         addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
//                     }
//                 }
//                 currentVelocity += addedVelocity;
//             }
//             currentVelocity += Gravity * deltaTime;
//             currentVelocity *= (1f / (1f + (Drag * deltaTime)));
//         }
//         //jumping
//         {
//             m_timeSinceJumpRequested += deltaTime;
//             if (m_jumpStatus == JumpStatus.Requested)
//             { 
//                 if (AllowWallJump&&m_canWallJump)
//                 {
//                     Vector3 jumpDirection = (m_wallJumpNormal+Motor.CharacterUp).normalized;
//                     Motor.ForceUnground(0.1f);
//                     currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
//                     currentVelocity += (m_moveInputVector * JumpScalableForwardSpeed);
//                     m_jumpStatus = JumpStatus.Executed;
//                     m_hasWallJumped = true;
//                     m_hasJumped = true;
//                 }
//                 else if (
//                     (AllowJumpingWhenSliding
//                              ? Motor.GroundingStatus.FoundAnyGround
//                              : Motor.GroundingStatus.IsStableOnGround) ||
//                          m_timeSinceJumpRequested <= JumpPreGroundingGraceTime)
//                 {
//                     Vector3 jumpDirection = Motor.CharacterUp;
//                     Motor.ForceUnground(0.1f);
//                     currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
//                     currentVelocity += (m_moveInputVector * JumpScalableForwardSpeed);
//                     m_jumpStatus = JumpStatus.Executed;
//                     m_hasJumped = true;
//                 }
//                 else if (!Motor.GroundingStatus.IsStableOnGround&& AllowDoubleJump)
//                 {
//                     Motor.ForceUnground(0.1f);
//                     currentVelocity += (Motor.CharacterUp * JumpSpeed)- Vector3.Project(currentVelocity, Motor.CharacterUp);
//                     m_jumpStatus = JumpStatus.DoubleJumpUsed;
//                     m_hasDoubleJumped = true;
//                 }
//             }
//             m_canWallJump = false;
//         }    
//     }
//
//     public void BeforeCharacterUpdate(float deltaTime)
//     {
//     }
//
//     public void PostGroundingUpdate(float deltaTime)
//     {
//         if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
//         {
//             OnLanded();
//         }else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
//         {
//             OnLeaveStableGround();
//         }
//     }
//
//     protected void OnLanded()
//     {
//         m_hasJumped = false;
//         m_hasDoubleJumped = false;
//         m_hasWallJumped = false;
//         m_jumpStatus= JumpStatus.None;
//         m_timeSinceJumpRequested = 0f;
//
//     }
//
//     protected void OnLeaveStableGround()
//     {
//         
//     }
//     public void AfterCharacterUpdate(float deltaTime)
//     {//reseting flags for jump
//         m_timeSinceJumpRequested += deltaTime;
//         if (m_jumpStatus == JumpStatus.Requested && m_timeSinceJumpRequested > JumpPreGroundingGraceTime)
//         {
//             m_jumpStatus = JumpStatus.None;
//         }
//
//         if (AllowJumpingWhenSliding
//                 ?Motor.GroundingStatus.FoundAnyGround:Motor.GroundingStatus.IsStableOnGround)
//         {
//             m_timeSinceLastAbleToJump = 0f;
//         }
//         else
//         {
//             m_timeSinceLastAbleToJump += deltaTime;
//         }
//     }
//
//     public bool IsColliderValidForCollisions(Collider coll)
//     {
//         return true;
//     }
//
//     public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
//     {
//     }
//
//     public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
//         ref HitStabilityReport hitStabilityReport)
//     {
//         //wall jump
//         if (AllowWallJump && Motor.GroundingStatus.IsStableOnGround&& !hitStabilityReport.IsStable)
//         {
//             m_canWallJump = true;
//             m_hasWallJumped=false;
//             m_wallJumpNormal=hitNormal;
//         }
//     }
//
//     public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
//         Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
//     {
//     }
//
//     public void OnDiscreteCollisionDetected(Collider hitCollider)
//     {
//     }
// }
