using Notifications;
using Player.Inventory;
using UnityEngine;

namespace Player {
	public class PlayerAutoResourceCollectorZone : MonoBehaviour {
		public GameObject player;
		private PlayerInventory _inventory;

		private void Start() {
			_inventory = player.GetComponent<PlayerInventory>();
		}

		private void OnTriggerEnter(Collider other) {
			if (!other.gameObject.TryGetComponent(out Loot.Loot loot)) return;

			if (_inventory.Collect(loot.kind, loot.amount, -1)) {
				Destroy(loot.gameObject);
				FindAnyObjectByType<NotificationManager>().ShowNotification("First steps!");
			}
		}
	}
}