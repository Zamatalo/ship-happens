using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Edible : MonoBehaviour, IUsableItem {
		private InputSystem_Actions _ia;
		private PlayerInventoryNew _inventory;
		private PlayerNeeds _needs;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		public void OnEnable() {
			_ia.Player.Enable();
			_ia.Player.UseItem.performed += Use;
		}

		public void OnDisable() {
			_ia.Player.UseItem.performed -= Use;
			_ia.Player.Disable();
		}

		public void Init(Transform origin, List<ItemParameter> parameters) {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponentInChildren<PlayerInventoryNew>();
			_needs = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerNeeds>();
		}

		private void Use(InputAction.CallbackContext context) {
			_needs.hunger = Mathf.Min(_needs.maxHunger, _needs.hunger + _needs.maxHunger / 4f);
			_inventory.RemoveItemServerRpc("Edible", 1);
		}
	}
}