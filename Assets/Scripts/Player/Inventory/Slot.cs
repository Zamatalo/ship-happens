using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Inventory {
	public class Slot : MonoBehaviour {
		public enum SlotKind {
			None,
			Magnet,
			HarpoonGun,
			Jetpack,
			Metal,
			Wood,
			Edible,
			Potable,
		}

		public TMP_Text amountText;
		public Image image;
		public Material magnetSprite;
		public Material harpoonGunSprite;
		public Material jetpackSprite;
		public Material metalSprite;
		public Material woodSprite;
		public Material edibleSprite;
		public Material potableSprite;
		public SlotKind kind;
		public int amount;

		private void Update() {
			switch (kind) {
				case SlotKind.None:
					image.material = null;
					image.enabled = false;
					break;
				case SlotKind.Magnet:
					image.enabled = true;
					image.material = magnetSprite;
					break;
				case SlotKind.HarpoonGun:
					image.enabled = true;
					image.material = harpoonGunSprite;
					break;
				case SlotKind.Jetpack:
					image.enabled = true;
					image.material = jetpackSprite;
					break;
				case SlotKind.Metal:
					image.enabled = true;
					image.material = metalSprite;
					break;
				case SlotKind.Wood:
					image.enabled = true;
					image.material = woodSprite;
					break;
				case SlotKind.Edible:
					image.enabled = true;
					image.material = edibleSprite;
					break;
				case SlotKind.Potable:
					image.enabled = true;
					image.material = potableSprite;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			amountText.text = $"{amount}";
		}

		public bool Stackable() {
			return kind switch {
				SlotKind.None => false,
				SlotKind.Magnet => false,
				SlotKind.HarpoonGun => false,
				SlotKind.Jetpack => false,
				SlotKind.Metal => true,
				SlotKind.Wood => true,
				SlotKind.Edible => true,
				SlotKind.Potable => true,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}