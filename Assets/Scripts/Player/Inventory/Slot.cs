using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Inventory {
	public class Slot : MonoBehaviour {
		public enum SlotKind {
			None,
			Magnet,
			Harpoon,
			Metal
		}

		public TMP_Text amountText;
		public Image image;
		public Material magnetSprite;
		public Material harpoonSprite;
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
				case SlotKind.Harpoon:
					image.enabled = true;
					image.material = harpoonSprite;
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
				SlotKind.Harpoon => false,
				SlotKind.Metal => true,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}