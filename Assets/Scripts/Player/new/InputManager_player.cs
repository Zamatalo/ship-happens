// using System;
// using System.Collections.Generic;
// using System.Numerics;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.TextCore.Text;
// using Quaternion = UnityEngine.Quaternion;
// using Vector2 = UnityEngine.Vector2;
// using Vector3 = UnityEngine.Vector3;
//
//
// public class InputManager_player : MonoBehaviour
// {
//     [SerializeField] private CharacterCamera _characterCamera;
//     [SerializeField] private LocomotionController _locomotionController;
//     [SerializeField] private Transform followPoint;
//
//     private Vector2 m_lookInput;
//     private Vector2 m_moveInput;
//     private Vector3 m_lookInputVector = Vector3.zero;
//     private bool m_jumpDown, m_jumpHeld;
//
//     public void Start()
//     {
//         _characterCamera.SetFollowTransform(followPoint);
//         _characterCamera.IgnoredColliders.Clear();
//
//
//     }
//
//     private void LateUpdate()
//     {
//         HandleCameraInput();
//         HandeCharacterInput();
//     }
//
//     private void HandleCameraInput()
//     {
//         var mouseLookAxisUp = m_lookInput.y;
//         var mouseLookAxisRight= m_lookInput.x;
//         //Prevent moving camera if cursor isnt locked
//         m_lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
//
//         if (Cursor.lockState == CursorLockMode.Locked)
//         {
//             m_lookInputVector = Vector3.zero;
//         }
//
//         var scrollInput = 0f;
//         
//         _characterCamera.UpdateWithInput(Time.deltaTime, scrollInput, m_lookInputVector);
//     }
//
//     private void HandeCharacterInput()
//     {
//         var inputs = new PlayerCharacterInputs
//         {
//             moveAxisForward = m_moveInput.y,
//             moveAxisRight = m_moveInput.x,
//             cameraRotation = _characterCamera.transform.rotation,
//             JumpDown = m_jumpDown,
//             JumpHeld = m_jumpHeld,
//         };
//         _locomotionController.SetInputs(ref inputs);
//         m_jumpDown = false;
//     }
//     
//     
//     public void OnLook(InputAction.CallbackContext context)
//     {
//         m_lookInput = context.ReadValue<Vector2>();
//     }
//     public void OnMove(InputAction.CallbackContext context)
//     {
//         m_moveInput = context.ReadValue<Vector2>();
//     }
//     public void OnJump(InputAction.CallbackContext context)
//     {
//         switch (context.phase)
//         {
//             case InputActionPhase.Started:
//                 m_jumpDown=true;
//                 break;
//             case InputActionPhase.Performed:
//                 m_jumpHeld=true;
//                 break;
//             case InputActionPhase.Canceled:
//                 m_jumpHeld=false;
//                 break;
//         }
//     }
//
// }
