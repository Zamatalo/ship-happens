using Settings.InputSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerController_Cinemachine : NetworkBehaviour {
		//---------------UTIL---------------
		private const float _threshold = 0.01f;

		//---------------MOVEMENT---------------
		public float moveSpeed = 5f;
		public float jumpForce = 5f;
		public float RotationSpeed = 1.0f;
		public float SpeedChangeRate = 10.0f;
		public float JumpHeight = 1.2f;
		public float Gravity = -15.0f;
		public float SprintSpeed = 6.0f;
		public float JumpTimeout = 0.1f;
		public float FallTimeout = 0.15f;

		//---------------GROUND---------------
		public bool isGrounded = true;
		public float GroundedOffset = -0.14f;
		public float GroundedRadius = 0.5f;
		public LayerMask GroundLayers;
		public Animator animator;
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float BottomClamp = -90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float TopClamp = 90.0f;
		//private Vector2 moveInput;
		//private Rigidbody rb;
		// public GameObject characterObj;
		// public GameObject handsObj;
		private StarterAssetsInputs _input;

		//---------------CAMERA---------------
		private float cinemachineTargetPitch;
		private CharacterController controller;
		private float fallTimeoutDelta;

		//---------------ANIMATOR---------------
		private bool isMoving;

		private float jumpTimeoutDelta;
		private GameObject mainCamera;

		private PlayerInput playerInput;

		//---------------OBJECTS---------------
		private InputSystem_Actions playerInputActions;
		private float rotationVelocity;
		private float speed;
		private readonly float terminalVelocity = 53.0f;
		private float verticalVelocity;

		private bool IsCurrentDeviceMouse {
			get {
#if ENABLE_INPUT_SYSTEM
				return playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
			}
		}

		private void Awake() {
			if (mainCamera == null) mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}


		private void Start() {
			animator = GetComponent<Animator>();
			controller = GetComponent<CharacterController>();
			playerInput = GetComponent<PlayerInput>();
			_input = GetComponent<StarterAssetsInputs>();

			// reset our timeouts on start
			jumpTimeoutDelta = JumpTimeout;
			fallTimeoutDelta = FallTimeout;
		}

		private void Update() {
			JumpAndGravity();
			GroundedCheck();
			Move();

			isMoving = _input.move.magnitude > 0.01f;
			animator.SetFloat("moveX", _input.move.x);
			animator.SetFloat("moveZ", _input.move.y);
			animator.SetBool("isGrounded", isGrounded);
			animator.SetBool("isMoving", isMoving);
		}

		private void LateUpdate() {
			CameraRotation();
		}

		private void OnDrawGizmosSelected() {
			var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (isGrounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(
				new Vector3(transform.position.x, transform.position.y - GroundedOffset,
					transform.position.z), GroundedRadius);
		}

		private void GroundedCheck() {
			// set sphere position, with offset
			var spherePosition = new Vector3(transform.position.x,
				transform.position.y - GroundedOffset, transform.position.z);
			isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
				QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation() {
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold) {
				//Don't multiply mouse input by Time.deltaTime
				var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation =
					Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * rotationVelocity);
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void Move() {
			// set target speed based on move speed, sprint speed and if sprint is pressed
			var targetSpeed = _input.sprint ? SprintSpeed : moveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			var currentHorizontalSpeed =
				new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

			var speedOffset = 0.1f;
			var inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset ||
			    currentHorizontalSpeed > targetSpeed + speedOffset) {
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
					Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				speed = Mathf.Round(speed * 1000f) / 1000f;
			} else {
				speed = targetSpeed;
			}

			// normalise input direction
			var inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
				// move
				inputDirection =
					transform.right * _input.move.x + transform.forward * _input.move.y;

			// move the player
			if (controller.enabled)
				controller.Move(inputDirection.normalized * (speed * Time.deltaTime) +
				                new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity() {
			if (isGrounded) {
				// reset the fall timeout timer
				fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (verticalVelocity < 0.0f) verticalVelocity = -2f;

				// Jump
				if (_input.jump && jumpTimeoutDelta <= 0.0f) {
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					animator.SetTrigger("Jump");
				}

				// jump timeout
				if (jumpTimeoutDelta >= 0.0f) jumpTimeoutDelta -= Time.deltaTime;
			} else {
				// reset the jump timeout timer
				jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (fallTimeoutDelta >= 0.0f) fallTimeoutDelta -= Time.deltaTime;

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (verticalVelocity < terminalVelocity) verticalVelocity += Gravity * Time.deltaTime;
		}
	}
}