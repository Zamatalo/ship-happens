using System.Collections.Generic;
using Notifications;
using Unity.Netcode;
using UnityEngine;

namespace Player {
	public class PlayerAutoResourceCollectorZone : NetworkBehaviour {
		private PlayerInventoryNew _inventory;
		private int _collectedCount = 0;
		private bool _shown1 = false;
		private bool _shown3 = false;
		private bool _shown5 = false;
		private bool _shown10 = false;
		private bool _shown30 = false;
		private bool _shown100 = false;


		private void OnTriggerEnter(Collider other) {
			if (!other.gameObject.TryGetComponent(out Loot.Loot loot)) return;

			_inventory.AddItemServerRpc(
				loot.kind.itemName,
				loot.kind.quantity,
				null
				);
			Destroy(loot.gameObject);
			_collectedCount++;
			ShowMilestoneNotification();
		}
		private void ShowMilestoneNotification() {
			var notifier = FindAnyObjectByType<NotificationManager>();

			if (_collectedCount >= 1 && !_shown1) {
				notifier.ShowNotification("First steps!");
				_shown1 = true;
			}
			else if (_collectedCount >= 3 && !_shown3) {
				notifier.ShowNotification("Nice work, keep going!");
				_shown3 = true;
			}
			else if (_collectedCount >= 5 && !_shown5) {
				notifier.ShowNotification("You're on a roll!");
				_shown5 = true;
			}
			else if (_collectedCount >= 10 && !_shown10) {
				notifier.ShowNotification("Serious collector!");
				_shown10 = true;
			}
			else if (_collectedCount >= 30 && !_shown30) {
				notifier.ShowNotification("Master Gatherer!");
				_shown30 = true;
			}
			else if (_collectedCount >= 100 && !_shown100) {
				notifier.ShowNotification("Legendary Hoarder!");
				_shown100 = true;
			}
		}

		protected override void OnNetworkPostSpawn() {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerInventoryNew>();
		}
	}
}