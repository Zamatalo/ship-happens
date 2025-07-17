using System;
using Item;
using Player.Inventory;
using UnityEngine;

namespace Player {
	public class PlayerItem : MonoBehaviour {
		public GameObject playerObject;
		public GameObject magnetPrefab;
		public GameObject harpoonGunPrefab;
		public GameObject metalPrefab;
		public Transform cameraTransform;
		private GameObject _current;
		private PlayerInventory _inventory;

		private void Start() {
			_current = Instantiate(magnetPrefab);
			_inventory = playerObject.GetComponent<PlayerInventory>();
			_inventory.AddSlotChangeListener(OnSlotChange);
		}

		private void OnSlotChange(Slot slot) {
			Destroy(_current);
			switch (slot.kind) {
				case Slot.SlotKind.None:
					break;
				case Slot.SlotKind.Magnet:
					_current = Instantiate(magnetPrefab, transform);
					_current.GetComponent<Magnet>().Init(cameraTransform);
					break;
				case Slot.SlotKind.HarpoonGun:
					_current = Instantiate(harpoonGunPrefab, transform);
					_current.GetComponent<HarpoonGun>().Init(cameraTransform);
					break;
				case Slot.SlotKind.Metal:
					_current = Instantiate(metalPrefab, transform);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}