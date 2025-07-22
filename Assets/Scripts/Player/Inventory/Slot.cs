// using System;
// using TMPro;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Player.Inventory {
// 	[Serializable]
// 	public class Slot : MonoBehaviour, INetworkSerializable {
// 		public enum SlotKind {
// 			None,
// 			Magnet,
// 			HarpoonGun,
// 			Jetpack,
// 			Metal,
// 			Planks,
// 			Edible,
// 			Potable,
// 		}
//
// 		public TMP_Text amountText;
// 		public Image image;
// 		public Material magnetSprite;
// 		public Material harpoonGunSprite;
// 		public Material jetpackSprite;
// 		public Material metalSprite;
// 		public Material planksSprite;
// 		public Material edibleSprite;
// 		public Material potableSprite;
// 		public string kind;
// 		public int amount;
// 		public float jetpackFuel = -1;
//
// 		private void Update() {
// 			switch (kind) {
// 				case SlotKind.None:
// 					image.material = null;
// 					image.enabled = false;
// 					break;
// 				case SlotKind.Magnet:
// 					image.enabled = true;
// 					image.material = magnetSprite;
// 					break;
// 				case SlotKind.HarpoonGun:
// 					image.enabled = true;
// 					image.material = harpoonGunSprite;
// 					break;
// 				case SlotKind.Jetpack:
// 					image.enabled = true;
// 					image.material = jetpackSprite;
// 					break;
// 				case SlotKind.Metal:
// 					image.enabled = true;
// 					image.material = metalSprite;
// 					break;
// 				case SlotKind.Planks:
// 					image.enabled = true;
// 					image.material = planksSprite;
// 					break;
// 				case SlotKind.Edible:
// 					image.enabled = true;
// 					image.material = edibleSprite;
// 					break;
// 				case SlotKind.Potable:
// 					image.enabled = true;
// 					image.material = potableSprite;
// 					break;
// 				default:
// 					throw new ArgumentOutOfRangeException();
// 			}
// 			amountText.text = $"{amount}";
// 		}
//
// 		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
// 			serializer.SerializeValue(ref kind);
// 			serializer.SerializeValue(ref amount);
// 			serializer.SerializeValue(ref jetpackFuel);
// 		}
//
// 		public bool Stackable() {
// 			return kind switch {
// 				SlotKind.None => false,
// 				SlotKind.Magnet => false,
// 				SlotKind.HarpoonGun => false,
// 				SlotKind.Jetpack => false,
// 				SlotKind.Metal => true,
// 				SlotKind.Planks => true,
// 				SlotKind.Edible => true,
// 				SlotKind.Potable => true,
// 				_ => throw new ArgumentOutOfRangeException()
// 			};
// 		}
// 	}
// }

