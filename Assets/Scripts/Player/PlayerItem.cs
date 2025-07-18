using System;
using Item;
using Player.Inventory;
using UnityEngine;

namespace Player {
	public class PlayerItem : MonoBehaviour {
		public GameObject playerObject;
		public GameObject magnetPrefab;
		public GameObject harpoonGunPrefab;
		public GameObject jetpackPrefab;
		public GameObject metalPrefab;
		public GameObject woodPrefab;
		public GameObject ediblePrefab;
		public GameObject potablePrefab;
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
				case Slot.SlotKind.Jetpack:
					_current = Instantiate(jetpackPrefab, transform);
					_current.GetComponent<Jetpack>().Init(playerObject.GetComponent<Rigidbody>());
					break;
				case Slot.SlotKind.Metal:
					_current = Instantiate(metalPrefab, transform);
					break;
				case Slot.SlotKind.Wood:
					_current = Instantiate(woodPrefab, transform);
					break;
				case Slot.SlotKind.Edible:
					_current = Instantiate(ediblePrefab, transform);
					break;
				case Slot.SlotKind.Potable:
					_current = Instantiate(potablePrefab, transform);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}