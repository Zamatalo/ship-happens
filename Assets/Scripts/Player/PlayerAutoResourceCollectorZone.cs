using Achievements;
using Unity.Netcode;
using UnityEngine;

namespace Player {
	public class PlayerAutoResourceCollectorZone : NetworkBehaviour {
		private PlayerInventoryNew _playerInventory;
		private int _collectedCount = 0;
		private bool _shown1 = false;
		private bool _shown3 = false;
		private bool _shown5 = false;
		private bool _shown10 = false;
		private bool _shown30 = false;
		private bool _shown100 = false;

		private void OnTriggerEnter(Collider other) {
			if (other.gameObject.TryGetComponent(out Loot.Loot loot)) AddLoot(loot);

			if (other.TryGetComponent(out CollectableItem collectable)) CollectItem(collectable);

		}
		private void CollectItem(CollectableItem collectable)
		{
			if (_playerInventory == null) return;
		
		}

		private void AddLoot(Loot.Loot loot)
		{
			_playerInventory.AddItemServerRpc(
				loot.kind.itemName,
				loot.kind.quantity,
				null
			);
			Destroy(loot.gameObject);
			_collectedCount++;
			ShowMilestoneAchievement();
		}
		private void ShowMilestoneAchievement() {
			var notifier = FindAnyObjectByType<AchievementManager>();

			if (_collectedCount >= 1 && !_shown1) {
				notifier.ShowAchievement("First steps!");
				_shown1 = true;
			}
			else if (_collectedCount >= 3 && !_shown3) {
				notifier.ShowAchievement("Nice work, keep going!");
				_shown3 = true;
			}
			else if (_collectedCount >= 5 && !_shown5) {
				notifier.ShowAchievement("You're on a roll!");
				_shown5 = true;
			}
			else if (_collectedCount >= 10 && !_shown10) {
				notifier.ShowAchievement("Serious collector!");
				_shown10 = true;
			}
			else if (_collectedCount >= 30 && !_shown30) {
				notifier.ShowAchievement("Master Gatherer!");
				_shown30 = true;
			}
			else if (_collectedCount >= 100 && !_shown100) {
				notifier.ShowAchievement("Legendary Hoarder!");
				_shown100 = true;
			}
		}
		
		public override void OnNetworkSpawn()
		{
			if (!IsOwner) return;
        
			_playerInventory = GetComponentInParent<PlayerInventoryNew>();
        
			if (_playerInventory == null)
			{
				Transform playerTransform = transform.parent;
				if (playerTransform != null)
				{
					_playerInventory = playerTransform.Find("Inventory")?.GetComponent<PlayerInventoryNew>();
				}
			}

			if (_playerInventory == null)
			{
				Debug.LogError("Inventory reference not found!", this);
			}
		}

	}
}