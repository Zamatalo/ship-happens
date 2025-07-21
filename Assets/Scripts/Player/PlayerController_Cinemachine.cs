using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;


public class PlayerController_Cinemachine : NetworkBehaviour
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
    // public GameObject characterObj;
    // public GameObject handsObj;
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
    public float BottomClamp = -90.0f;
    public float TopClamp = 90.0f;

    //---------------UTIL---------------
    private const float _threshold = 0.01f;

    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    private float rotationVelocity;
    private float verticalVelocity;
    private float speed;
    

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }


    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<StarterAssetsInputs>();
        
        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
     
        isMoving = _input.move.magnitude > 0.01f;
        animator.SetFloat("moveX", _input.move.x);
        animator.SetFloat("moveZ", _input.move.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isMoving", isMoving);
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<PlayerInventoryNew>(out var inventory))
            {
                Debug.Log("Trying to add to player inventory");
                inventory.AddItemServerRpc("Poison", 1);
                inventory.AddItemServerRpc("Metal", 1);
                inventory.AddItemServerRpc("Harpoon", 1);
                inventory.AddItemServerRpc("Magnet", 1);
            }
        }
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
    
    private void CameraRotation()
    {
        if (!(_input.look.sqrMagnitude >= _threshold)) return;
        float deltaTimeMultiplier = Time.deltaTime;

        cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
        rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * rotationVelocity);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    
    private void Move()
    {
        float targetSpeed = _input.sprint ? SprintSpeed : moveSpeed;
        
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;


        if (_input.move != Vector2.zero)
        {
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        if (controller.enabled)
        {
            controller.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

    }
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            fallTimeoutDelta = FallTimeout;

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            if (_input.jump && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                animator.SetTrigger("Jump");

            }
            
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = JumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            _input.jump = false;
        }

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

			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
