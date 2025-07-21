using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimUI.Modern_Menu_1.Scripts.ThemeEditor {
	[Serializable]
	public class ThemedUIElement : ThemedUI {
		public enum OutlineStyle {
			solidThin,
			solidThick,
			dottedThin,
			dottedThick
		}

		public bool hasImage = false;
		public bool isText = false;
		private Image image;
		private GameObject message;
		[Header("Parameters")] private Color outline;

		protected override void OnSkinUI() {
			base.OnSkinUI();

			if (hasImage) {
				image = GetComponent<Image>();
				image.color = themeController.currentColor;
			}

			message = gameObject;

			if (isText) {
				message.GetComponent<TextMeshPro>().color = themeController.textColor;
			}
		}
	}
}