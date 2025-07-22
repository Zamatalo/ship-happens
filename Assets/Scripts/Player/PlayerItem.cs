// using System;
// using Item;
// using Item.Collectable;
// using Player.Inventory;
// using Unity.Netcode;
// using UnityEngine;
//
// namespace Player {
// 	public class PlayerItem : NetworkBehaviour {
// 		public GameObject playerObject;
// 		public GameObject magnetPrefab;
// 		public GameObject magnetCollectablePrefab;
// 		public GameObject harpoonGunPrefab;
// 		public GameObject harpoonGunCollectablePrefab;
// 		public GameObject jetpackPrefab;
// 		public GameObject jetpackCollectablePrefab;
// 		public GameObject metalPrefab;
// 		public GameObject metalCollectablePrefab;
// 		public GameObject planksPrefab;
// 		public GameObject planksCollectablePrefab;
// 		public GameObject ediblePrefab;
// 		public GameObject edibleCollectablePrefab;
// 		public GameObject potablePrefab;
// 		public GameObject potableCollectablePrefab;
// 		public Transform cameraTransform;
// 		public float itemDropForwardForce;
// 		public float itemDropUpForce;
// 		private GameObject _current;
// 		private PlayerInventory _inventory;
//
// 		private void Start() {
// 			if (!IsOwner) return;
//
// 			_current = Instantiate(magnetPrefab);
// 			_inventory = playerObject.GetComponent<PlayerInventory>();
// 			_inventory.AddSlotChangeListener(OnSlotChange);
// 			_inventory.AddDropItemListener(OnDropItem);
// 		}
//
// 		private void OnSlotChange(Slot slot) {
// 			if (!IsOwner) return;
//
// 			Destroy(_current);
// 			switch (slot.kind) {
// 				case Slot.SlotKind.None:
// 					break;
// 				case Slot.SlotKind.Magnet:
// 					_current = Instantiate(magnetPrefab, transform);
// 					_current.GetComponent<Magnet>().Init(cameraTransform);
// 					break;
// 				case Slot.SlotKind.HarpoonGun:
// 					_current = Instantiate(harpoonGunPrefab, transform);
// 					_current.GetComponent<HarpoonGun>().Init(cameraTransform);
// 					break;
// 				case Slot.SlotKind.Jetpack:
// 					var rb = playerObject.GetComponent<Rigidbody>();
//
// 					_current = Instantiate(jetpackPrefab, transform);
// 					_current.GetComponent<Jetpack>().Init(rb, cameraTransform, slot);
// 					break;
// 				case Slot.SlotKind.Metal:
// 					_current = Instantiate(metalPrefab, transform);
// 					break;
// 				case Slot.SlotKind.Planks:
// 					_current = Instantiate(planksPrefab, transform);
// 					break;
// 				case Slot.SlotKind.Edible: {
// 					var needs = playerObject.GetComponent<PlayerNeeds>();
// 					var inventory = playerObject.GetComponent<PlayerInventory>();
//
// 					_current = Instantiate(ediblePrefab, transform);
// 					_current.GetComponent<Edible>().Init(needs, inventory, slot);
// 					break;
// 				}
// 				case Slot.SlotKind.Potable: {
// 					var needs = playerObject.GetComponent<PlayerNeeds>();
// 					var inventory = playerObject.GetComponent<PlayerInventory>();
//
// 					_current = Instantiate(potablePrefab, transform);
// 					_current.GetComponent<Potable>().Init(needs, inventory, slot);
// 					break;
// 				}
// 				default:
// 					throw new ArgumentOutOfRangeException();
// 			}
// 		}
//
// 		private void OnDropItem(Slot slot) {
// 			if (!IsOwner) return;
//
// 			var item = slot.kind switch {
// 				Slot.SlotKind.None => throw new Exception("Unexpected slot kind"),
// 				Slot.SlotKind.Magnet => Instantiate(magnetCollectablePrefab, transform.position,
// 					Quaternion.identity),
// 				Slot.SlotKind.HarpoonGun => Instantiate(harpoonGunCollectablePrefab,
// 					transform.position, Quaternion.identity),
// 				Slot.SlotKind.Jetpack => Instantiate(jetpackCollectablePrefab, transform.position,
// 					Quaternion.identity),
// 				Slot.SlotKind.Metal => Instantiate(metalCollectablePrefab, transform.position,
// 					Quaternion.identity),
// 				Slot.SlotKind.Planks => Instantiate(planksCollectablePrefab, transform.position,
// 					Quaternion.identity),
// 				Slot.SlotKind.Edible => Instantiate(edibleCollectablePrefab, transform.position,
// 					Quaternion.identity),
// 				Slot.SlotKind.Potable => Instantiate(potableCollectablePrefab, transform.position,
// 					Quaternion.identity),
// 				_ => throw new ArgumentOutOfRangeException()
// 			};
//
// 			var force = cameraTransform.forward * itemDropForwardForce +
// 			            cameraTransform.up * itemDropUpForce;
// 			item.GetComponent<CollectableItem>().Init(slot.kind, slot.amount, slot.JetpackFuel);
// 			item.GetComponent<Rigidbody>().AddForce(force);
// 		}
// 	}
// }