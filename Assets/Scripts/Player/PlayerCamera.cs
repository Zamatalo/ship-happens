using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerCamera : NetworkBehaviour
	{
		[Header("Settings")]
		public float mouseSensitivity = 100f;
		public Transform playerBody;

		private float xRotation = 0f;
		private InputSystem_Actions inputActions;
		private Vector2 lookInput;

		private Camera playerCamera;

		private void Awake()
		{
			inputActions = new InputSystem_Actions();
			playerCamera = GetComponentInChildren<Camera>();

			if (playerBody == null)
			{
				playerBody = transform.parent;
				if (playerBody == null)
				{
				}
			}
		}

		public override void OnNetworkSpawn()
		{
			if (!IsOwner)
			{
				if (playerCamera != null) playerCamera.enabled = false;
				enabled = false;
				return;
			}

			Cursor.lockState = CursorLockMode.Locked;

			inputActions.Enable();
			inputActions.Player.Look.performed += OnLook;
			inputActions.Player.Look.canceled += OnLook;
		}

		public override void OnNetworkDespawn()
		{
			if (IsOwner)
			{
				inputActions.Player.Look.performed -= OnLook;
				inputActions.Player.Look.canceled -= OnLook;
				inputActions.Disable();
			}
		}

		private void OnLook(InputAction.CallbackContext context)
		{
			lookInput = context.ReadValue<Vector2>();
		}

		private void Update()
		{
			if (!IsOwner) return;

			float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
			float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

			xRotation += mouseY;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);

			transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

			if (playerBody != null)
			{
				playerBody.Rotate(Vector3.up * mouseX);
			}
		}
	}
}