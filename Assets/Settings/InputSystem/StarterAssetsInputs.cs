using UnityEngine;
using UnityEngine.InputSystem;

namespace Settings.InputSystem {
	public class StarterAssetsInputs : MonoBehaviour {
		private static bool m_FocusActionsSetUp;
		[Header("Character Input Values")] public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")] public bool analogMovement;

		[Header("Mouse Cursor Settings")] public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		private InputSystem_Actions _ia;


		private bool m_IgnoreInput;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			if (!m_FocusActionsSetUp) {
#if UNITY_EDITOR
				var ignoreInput = new InputAction(binding: "/Keyboard/escape");
				ignoreInput.performed += context => m_IgnoreInput = true;
				ignoreInput.Enable();

				var enableInput = new InputAction(binding: "/Mouse/leftButton");
				enableInput.performed += context => m_IgnoreInput = false;
				enableInput.Enable();
#endif

				m_FocusActionsSetUp = true;
			}
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Player.Sprint.performed += OnSprintStart;
			_ia.Player.Sprint.canceled += OnSprintEnd;
		}

		private void OnDisable() {
			_ia.Player.Sprint.performed -= OnSprintStart;
			_ia.Player.Sprint.canceled -= OnSprintEnd;
			_ia.Disable();
		}

		private void OnDestroy() {
			m_FocusActionsSetUp = false;
		}

		private void OnApplicationFocus(bool hasFocus) {
			SetCursorState(cursorLocked);
			m_IgnoreInput = !hasFocus;
		}

		private void OnSprintStart(InputAction.CallbackContext _) {
			sprint = true;
		}

		private void OnSprintEnd(InputAction.CallbackContext _) {
			sprint = false;
		}


		public void MoveInput(Vector2 newMoveDirection) {
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection) {
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState) {
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState) {
			sprint = newSprintState;
		}

		private void SetCursorState(bool newState) {
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value) {
			if (m_IgnoreInput) {
				MoveInput(Vector2.zero);
				return;
			}

			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value) {
			if (m_IgnoreInput) {
				LookInput(Vector2.zero);
				return;
			}

			if (cursorInputForLook) {
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value) {
			JumpInput(value.isPressed);
		}

		// public void OnSprint(InputValue value)
		// {
		// 	SprintInput(value.isPressed);
		// }
#endif
	}
}