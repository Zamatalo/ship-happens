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
			Metal
		}

		public TMP_Text amountText;
		public Image image;
		public Material magnetSprite;
		public Material harpoonGunSprite;
		public Material jetpackSprite;
		public Material metalSprite;
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
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}