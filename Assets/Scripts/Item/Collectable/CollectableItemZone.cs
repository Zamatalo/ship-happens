using Player.Inventory;
using UnityEngine;

namespace Item.Collectable {
	[RequireComponent(typeof(SphereCollider))]
	public class CollectableItemZone : MonoBehaviour {
		public delegate void OnZoneEnter(PlayerInventory inventory);
		private OnZoneEnter _onZoneEnter;

		private void Start() {
			GetComponent<SphereCollider>().radius = 0.5f;
		}

		private void OnTriggerEnter(Collider other) {
			if (_onZoneEnter == null) return;

			if (!other.gameObject.TryGetComponent(out PlayerInventory inventory)) return;
			_onZoneEnter.Invoke(inventory);
		}

		public void Init(OnZoneEnter onZoneEnter) {
			_onZoneEnter = onZoneEnter;
		}
	}
}