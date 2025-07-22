// using Player.Inventory;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// namespace Player {
// 	public class PlayerController_old : NetworkBehaviour {
// 		public float moveSpeed = 5f;
// 		public float jumpForce = 5f;
// 		public GameObject characterObj;
// 		public GameObject handsObj;
// 		public bool isGrounded;
// 		public Animator animator;
// 		public Transform foot;
// 		private bool _isMoving;
// 		private Vector2 _moveInput;
// 		private InputSystem_Actions _playerInputActions;
// 		private PlayerInventory _playerInventory;
// 		private Rigidbody _rb;
// 		private Slot _slot;
//
// 		private void Awake() {
// 			_playerInputActions = new InputSystem_Actions();
// 		}
//
// 		private void Start() {
// 			_rb = GetComponent<Rigidbody>();
// 			animator = GetComponent<Animator>();
// 			
// 			if (IsLocalPlayer) {
// 				characterObj.SetActive(false);
// 				handsObj.SetActive(true);
// 			} else {
// 				characterObj.SetActive(true);
// 				handsObj.SetActive(false);
// 			}
// 		}
//
// 		private void FixedUpdate()
// 		{
// 			if (!IsOwner) return;
// 			
// 			isGrounded = Physics.Raycast(foot.position, transform.up * -1f, 0.1f);
//
// 			var inputDir = new Vector3(_moveInput.x, 0f, _moveInput.y);
//
// 			var movement = transform.rotation * inputDir;
// 			movement *= moveSpeed * Time.fixedDeltaTime;
// 			_rb.MovePosition(_rb.position + movement);
//
// 			_isMoving = _moveInput.sqrMagnitude > 0.01f;
// 			animator.SetFloat("moveX", _moveInput.x);
// 			animator.SetFloat("moveZ", _moveInput.y);
// 			animator.SetBool("isGrounded", isGrounded);
// 			animator.SetBool("isMoving", _isMoving);
// 		}
//
// 		private void OnEnable() {
// 			_playerInputActions.Player.Enable();
// 			_playerInputActions.Player.Move.performed += OnMove;
// 			_playerInputActions.Player.Move.canceled += OnMove;
// 			_playerInputActions.Player.Jump.performed += OnJump;
// 		}
//
// 		private void OnDisable() {
// 			_playerInputActions.Player.Jump.performed -= OnJump;
// 			_playerInputActions.Player.Move.canceled -= OnMove;
// 			_playerInputActions.Player.Move.performed -= OnMove;
// 			_playerInputActions.Player.Disable();
// 		}
//
// 		private void OnMove(InputAction.CallbackContext context) {
// 			if (!IsOwner) return;
// 			_moveInput = context.ReadValue<Vector2>();
// 		}
//
// 		private void OnJump(InputAction.CallbackContext context) {
// 			if (!IsOwner) return;
// 			if (isGrounded) Jump();
// 		}
//
// 		private void Jump() {
// 			_rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
// 			animator.SetTrigger("Jump");
// 		}
// 	}
// }

