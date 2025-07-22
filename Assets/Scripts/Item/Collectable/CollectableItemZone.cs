using UnityEngine;

namespace Item.Collectable {
	[RequireComponent(typeof(SphereCollider))]
	public class CollectableItemZone : MonoBehaviour {
		public delegate void OnZoneEnter(PlayerInventoryNew inventory);
		private OnZoneEnter _onZoneEnter;

		private void Start() {
			GetComponent<SphereCollider>().radius = 0.5f;
		}

		private void OnTriggerEnter(Collider other) {
			if (_onZoneEnter == null) return;

			if (!other.gameObject.TryGetComponent(out PlayerInventoryNew inventory)) return;
			_onZoneEnter.Invoke(inventory);
		}

		public void Init(OnZoneEnter onZoneEnter) {
			_onZoneEnter = onZoneEnter;
		}
	}
}