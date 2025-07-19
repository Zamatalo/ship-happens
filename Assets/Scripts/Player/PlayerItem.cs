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
		public GameObject planksPrefab;
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
				case Slot.SlotKind.Planks:
					_current = Instantiate(planksPrefab, transform);
					break;
				case Slot.SlotKind.Edible: {
					var needs = playerObject.GetComponent<PlayerNeeds>();
					var inventory = playerObject.GetComponent<PlayerInventory>();

					_current = Instantiate(ediblePrefab, transform);
					_current.GetComponent<Edible>().Init(needs, inventory, slot);
					break;
				}
				case Slot.SlotKind.Potable: {
					var needs = playerObject.GetComponent<PlayerNeeds>();
					var inventory = playerObject.GetComponent<PlayerInventory>();

					_current = Instantiate(potablePrefab, transform);
					_current.GetComponent<Potable>().Init(needs, inventory, slot);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}