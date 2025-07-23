using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerController : NetworkBehaviour {
		public Transform ItemHolder;

		[Header("Movement")] public float moveSpeed = 5f;

		[Header("Jump")] public float jumpForce = 5f;

		[Header("Ground")] public bool isGrounded = true;
		public float GroundedOffset = -0.14f;
		public float GroundedRadius = 0.5f;
		public LayerMask GroundLayers;
		public GameObject CinemachineCameraTarget;
		public float BottomClamp = -90.0f;
		public float TopClamp = 90.0f;
		public float RotationSpeed = 1.0f;
		private bool _isMoving;
		private Vector2 _moveInput;
		
		
		[Header("Items")] private PlayerInventoryNew _playerInventory;
		private Rigidbody _rb;

		[Header("Animation")] private Animator animator;

		[Header("Camera")] private float cinemachineTargetPitch;
		private IUsableItem currentItemScript;

		private Vector3 m_lookInput;
		private GameObject mainCamera;
		private float rotationVelocity;

		private void FixedUpdate() {
			if (!IsOwner) return;

			GroundedCheck();

			var inputDir = new Vector3(_moveInput.x, 0f, _moveInput.y);

			var movement = transform.rotation * inputDir;
			movement *= moveSpeed * Time.fixedDeltaTime;
			_rb.MovePosition(_rb.position + movement);

			_isMoving = _moveInput.sqrMagnitude > 0.01f;
			animator.SetFloat("moveX", _moveInput.x);
			animator.SetFloat("moveZ", _moveInput.y);
			animator.SetBool("isGrounded", isGrounded);
			animator.SetBool("isMoving", _isMoving);
		}

		private void LateUpdate() {
			CameraRotation();
		}

		private void OnDrawGizmos() {
			Gizmos.DrawSphere(transform.position + Vector3.down * GroundedOffset, GroundedRadius);
		}

		public override void OnNetworkSpawn() {
			_rb = GetComponent<Rigidbody>();
			animator = GetComponent<Animator>();

			var spawnObj = GameObject.FindGameObjectWithTag("SpawnBlock");
			if (spawnObj != null)
				transform.position = spawnObj.transform.position;
			else
				Debug.LogWarning("SpawnBlock not found! Spawning at default position.");
		}


		protected override void OnNetworkPostSpawn() {
			_playerInventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerInventoryNew>();
		}

		private void CameraRotation() {
			var deltaTimeMultiplier = Time.deltaTime;

			cinemachineTargetPitch += m_lookInput.y * RotationSpeed * deltaTimeMultiplier;
			rotationVelocity = m_lookInput.x * RotationSpeed * deltaTimeMultiplier;

			cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
			CinemachineCameraTarget.transform.localRotation =
				Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
			transform.Rotate(Vector3.up * rotationVelocity);
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void GroundedCheck() {
			var spherePosition = transform.position + Vector3.down * GroundedOffset;
			isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
				QueryTriggerInteraction.Ignore);
		}

		public void Jump() {
			_rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
			animator.SetTrigger("Jump");
		}

		public void OnMove(InputAction.CallbackContext context) {
			if (!IsOwner) return;
			_moveInput = context.ReadValue<Vector2>();
		}

		public void OnJump(InputAction.CallbackContext context) {
			if (!IsOwner) return;
			if (isGrounded) Jump();
		}

		public void OnLook(InputAction.CallbackContext context) {
			if (!IsOwner) return;
			m_lookInput = context.ReadValue<Vector2>();
		}

		public void OnAddAllItems(InputAction.CallbackContext context) {
			if (!IsOwner) return;

			if (_playerInventory == null) {
				Debug.LogWarning("PlayerInventoryNew not initialized");
				return;
			}
			_playerInventory.AddItemServerRpc("BuildingTool", 1,null);
			_playerInventory.AddItemServerRpc("Jetpack", 1,null);
			_playerInventory.AddItemServerRpc("Edible", 1,null);
			_playerInventory.AddItemServerRpc("GravityGun", 1,null);
			_playerInventory.AddItemServerRpc("HarpoonGun", 1,null);
			_playerInventory.AddItemServerRpc("Jetpack", 1,null);
			_playerInventory.AddItemServerRpc("Metal", 1,null);
			_playerInventory.AddItemServerRpc("Planks", 1,null);
			_playerInventory.AddItemServerRpc("Poison", 1,null);
			_playerInventory.AddItemServerRpc("Potable", 1,null);
			_playerInventory.AddItemServerRpc("Tablet", 1,null);
		}

		public void UpdateEquippedItem() {
			if (ItemHolder.childCount > 0) {
				currentItemScript = ItemHolder.GetChild(0).GetComponent<IUsableItem>();
			} else {
				currentItemScript = null;
			}
		}


		public void OnUseItem(InputAction.CallbackContext context) {
			if (!IsOwner) return;

			if (context.started) {
				UpdateEquippedItem();
				currentItemScript?.Use();
			} else if (context.canceled) {
				currentItemScript?.Unuse();
			}
		}
	}
}