using System.Collections;
using Player.Inventory;
using UnityEngine;

namespace Item.Collectable {
	[RequireComponent(typeof(Rigidbody))]
	public class CollectableItem : MonoBehaviour {
		public GameObject collectZoneObject;
		private int _amount;
		private float? _jetpackFuel;
		private Slot.SlotKind _kind;
		private Rigidbody _rb;
		private CollectableItemZone _zone;

		private void Start() {
			_zone = collectZoneObject.GetComponent<CollectableItemZone>();
			_rb = GetComponent<Rigidbody>();
			StartCoroutine(AfterStart());
		}

		public void Init(Slot.SlotKind kind, int amount, float? jetpackFuel) {
			_kind = kind;
			_amount = amount;
			_jetpackFuel = jetpackFuel;
		}

		private IEnumerator AfterStart() {
			yield return new WaitForSeconds(1);

			_rb.constraints = RigidbodyConstraints.FreezeAll;
			_zone.Init(OnZoneEnter);
		}

		private void OnZoneEnter(PlayerInventory inventory) {
			if (inventory.Collect(_kind, _amount, _jetpackFuel)) Destroy(gameObject);
		}
	}
}