using Notifications;
using Unity.Netcode;
using UnityEngine;

namespace Player {
	public class PlayerAutoResourceCollectorZone : NetworkBehaviour {
		private PlayerInventoryNew _inventory;

		private void OnTriggerEnter(Collider other) {
			if (!other.gameObject.TryGetComponent(out Loot.Loot loot)) return;

			_inventory.AddItemServerRpc(loot.kind.itemName, loot.kind.quantity);
			Destroy(loot.gameObject);
			FindAnyObjectByType<NotificationManager>().ShowNotification("First steps!");
		}

		protected override void OnNetworkPostSpawn() {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerInventoryNew>();
		}
	}
}