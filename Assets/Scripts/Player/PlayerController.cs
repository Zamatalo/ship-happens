using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;


public class PlayerController : NetworkBehaviour
{

    //---------------MOVEMENT---------------
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float RotationSpeed = 1.0f;
    public float SpeedChangeRate = 10.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float SprintSpeed = 6.0f;
    public float JumpTimeout = 0.1f;
    private float terminalVelocity = 53.0f;
    public float FallTimeout = 0.15f;

    //---------------OBJECTS---------------
    private InputSystem_Actions playerInputActions;
    private CharacterController controller;
    //private Vector2 moveInput;
    //private Rigidbody rb;
    public GameObject characterObj;
    public GameObject handsObj;
    private StarterAssetsInputs _input;

    private PlayerInput playerInput;

    //---------------GROUND---------------
    public bool isGrounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.5f;
    public LayerMask GroundLayers;

    //---------------ANIMATOR---------------
    private bool isMoving;
    public Animator animator;

    //---------------CAMERA---------------
    private float cinemachineTargetPitch;
    public GameObject CinemachineCameraTarget;
    private GameObject mainCamera;
    [Tooltip("How far in degrees can you move the camera up")]
    public float BottomClamp = -90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float TopClamp = 90.0f;

    //---------------UTIL---------------
    private const float _threshold = 0.01f;

    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    private float rotationVelocity;
    private float verticalVelocity;
    private float speed;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<StarterAssetsInputs>();

        if (IsLocalPlayer)
        {
            characterObj.SetActive(false);
            handsObj.SetActive(true);
        }
        else
        {
            characterObj.SetActive(true);
            handsObj.SetActive(false);
        }
        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    // private void OnEnable()
    // {
    //     _playerInputActions.Player.Enable();
    //     _playerInputActions.Player.Move.performed += OnMove;
    //     _playerInputActions.Player.Move.canceled += OnMove;
    //     _playerInputActions.Player.Jump.performed += OnJump;
    // }

    // private void OnDisable()
    // {
    //     _playerInputActions.Player.Jump.performed -= OnJump;
    //     _playerInputActions.Player.Move.canceled -= OnMove;
    //     _playerInputActions.Player.Move.performed -= OnMove;
    //     _playerInputActions.Player.Disable();
    // }

    // private void OnMove(InputAction.CallbackContext context)
    // {
    //   //  if (!IsOwner) { return;}
    //     moveInput = context.ReadValue<Vector2>();
    // }

    // private void OnJump(InputAction.CallbackContext context)
    // {
    //     //if (!IsOwner) { return;}
    //     if (isGrounded)
    //     {
    //         Jump();
    //     }
    // }

    // private void Jump()
    // {
    //     rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //     animator.SetTrigger("Jump");
    // }

    // private void FixedUpdate()
    // {
    //     //if (IsOwner)
    //     {
    //         Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

    //         Vector3 movement = (transform.rotation) * inputDir;
    //         movement *= moveSpeed * Time.fixedDeltaTime;
    //         rb.MovePosition(rb.position + movement);

    //         isMoving = moveInput.sqrMagnitude > 0.01f;
    //         animator.SetFloat("moveX", moveInput.x);
    //         animator.SetFloat("moveZ", moveInput.y);
    //         animator.SetFloat("verticalSpeed", rb.linearVelocity.y);
    //         animator.SetBool("isGrounded", isGrounded);
    //         animator.SetBool("isMoving", isMoving);
    //     }
    // }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = false;
    //     }
    // }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
        if (_input.jump) Debug.Log("Jump pressed!");
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
    private void CameraRotation()
    {
        // if there is an input
        if (_input.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            // move
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        // move the player
        if (controller.enabled)
        {
            controller.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

    }
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }
        private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (isGrounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
