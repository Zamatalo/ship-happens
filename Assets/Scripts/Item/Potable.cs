using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Item {
	public class Potable : MonoBehaviour, IUsableItem {
		private PlayerInventoryNew _inventory;
		private PlayerNeeds _needs;

		public void Init(Transform origin, List<ItemParameter> parameters) {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerInventoryNew>();
			_needs = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerNeeds>();
		}

		public void Use() {
			_needs.thirst = Mathf.Min(_needs.maxThirst, _needs.thirst + _needs.maxThirst / 3f);
			_inventory.RemoveItemServerRpc("Potable", 1);
		}

		public void Unuse() {
		}
	}
}