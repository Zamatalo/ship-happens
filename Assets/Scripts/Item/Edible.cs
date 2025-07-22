using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Item {
	public class Edible : MonoBehaviour, IUsableItem {
		private PlayerInventoryNew _inventory;
		private PlayerNeeds _needs;

		public void Init(Transform origin, List<ItemParameter> parameters) {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerInventoryNew>();
			_needs = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerNeeds>();
		}

		public void Use() {
			_needs.hunger = Mathf.Min(_needs.maxHunger, _needs.hunger + _needs.maxHunger / 4f);
			_inventory.RemoveItemServerRpc("Edible", 1);
		}

		public void Unuse() {
		}
	}
}