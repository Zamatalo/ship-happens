using System;
using UnityEngine;

namespace SlimUI.Modern_Menu_1.Scripts.ThemeEditor {
	[ExecuteInEditMode()]
	[Serializable]
	public class ThemedUI : MonoBehaviour {
		public ThemedUIData themeController;

		public virtual void Awake() {
			OnSkinUI();
		}

		public virtual void Update() {
			OnSkinUI();
		}

		protected virtual void OnSkinUI() {
		}
	}
}