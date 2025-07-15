using System;
using Inventory;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour {
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private InputSystem_Actions _playerInputActions;
    private Vector2 moveInput;
    private Rigidbody rb;
    public GameObject characterObj;
    public GameObject handsObj;
    public GameObject itemObj;
    public bool isGrounded;
    private bool isMoving;
    public Animator animator;
    private Inventory.Inventory _inventory;
    private Inventory.Slot _slot;
    public Transform foot;
   
        private void Awake() {
            _playerInputActions = new InputSystem_Actions();
        }

        private void Start() {
            _inventory = FindAnyObjectByType<Inventory.Inventory>();
            _inventory.AddSlotChangeListener(OnSlotChange);
            
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
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
        }

        private void OnEnable()
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Move.performed += OnMove;
            _playerInputActions.Player.Move.canceled += OnMove;
            _playerInputActions.Player.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Jump.performed -= OnJump;
            _playerInputActions.Player.Move.canceled -= OnMove;
            _playerInputActions.Player.Move.performed -= OnMove;
            _playerInputActions.Player.Disable();
        }

        private void OnSlotChange(Slot slot) {
            _slot = slot;
            switch (_slot.kind) {
                case Slot.SlotKind.NONE:
                    itemObj.SetActive(false);
                    break;
                case Slot.SlotKind.HOOK:
                    itemObj.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            //  if (!IsOwner) { return;}
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            //if (!IsOwner) { return;}
            if (isGrounded)
            {
                Jump();
            }
        }

        private void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

    private void FixedUpdate()
    {
        
        if (IsOwner)
        {
            isGrounded = Physics.Raycast(foot.position, Vector3.down, 0.1f);
            
            var inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

            Vector3 movement = (transform.rotation) * inputDir;
            movement *= moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            isMoving = moveInput.sqrMagnitude > 0.01f;
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveZ", moveInput.y);
            animator.SetFloat("verticalSpeed", rb.linearVelocity.y);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isMoving", isMoving);
        }
    }
}