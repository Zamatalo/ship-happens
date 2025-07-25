using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class OxygenGenerator : MonoBehaviour, IUsableItem {
		public GameObject prefab;
		public float maxPlaceDist = 20f;
		private Transform _camera;
		private InputSystem_Actions _ia;
		private PlayerInventoryNew _inventory;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void OnEnable() {
			_ia.Player.UseItem.performed += OnPlace;
			_ia.Enable();
		}

		private void OnDisable() {
			_ia.Player.UseItem.performed -= OnPlace;
			_ia.Disable();
		}

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) {
			_camera = cameraTransform;
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponentInChildren<PlayerInventoryNew>();
		}

		private void OnPlace(InputAction.CallbackContext _) {
			if (!Physics.Raycast(_camera.position, _camera.forward, out var hit, maxPlaceDist))
				return;
			Instantiate(prefab, hit.point, Quaternion.Euler(hit.normal));
			_inventory.RemoveItemServerRpc("OxygenGenerator", 1);
		}
	}
}