using System.Collections.Generic;
using Achievements;
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
		[SerializeField] private float collectRadius = 2f;
		private Collider[] _overlapResults = new Collider[20]; 

		private void Update()
		{
			if (!IsOwner || !_inventory) return;

			int count = Physics.OverlapSphereNonAlloc(transform.position, collectRadius, _overlapResults);

			for (int i = 0; i < count; i++)
			{
				var hit = _overlapResults[i];

				if (!hit) continue;

				if (hit.gameObject.TryGetComponent(out Loot.Loot loot))
				{
					AddLoot(loot);
					continue;
				}

				if (hit.TryGetComponent(out CollectableItem collectable))
				{
					if (collectable.GetParameters() != null) 
					{
						CollectItem(collectable);
					}
				}
			}
		}


		private void CollectItem(CollectableItem collectable)
		{
			if (!_inventory) return;
			var parms = collectable.GetParameters();
			if (parms == null) return;
			
			var arrparms = parms.ToArray();

			_inventory.AddItemServerRpc(collectable.GetItemName(), 1, arrparms);

			if (collectable.TryGetComponent(out NetworkObject netObj))
			{
				if (IsServer)
					netObj.Despawn(true);
			}
			else
			{
				Destroy(collectable.gameObject);
			}

			_collectedCount++;
			ShowMilestoneAchievement();
		}
		private void AddLoot(Loot.Loot loot)
		{
			_inventory.AddItemServerRpc(
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
        
			_inventory = GetComponentInParent<PlayerInventoryNew>();
        
			if (_inventory == null)
			{
				Transform playerTransform = transform.parent;
				if (playerTransform != null)
				{
					_inventory = playerTransform.Find("Inventory")?.GetComponent<PlayerInventoryNew>();
				}
			}

			if (_inventory == null)
			{
				Debug.LogError("Inventory reference not found!", this);
			}
		}
	}
}