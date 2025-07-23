using System.Collections.Generic;
using UnityEngine;

namespace Item {
	public class UnusableItem : MonoBehaviour, IUsableItem {
		public string name;

		public string Name() {
			return name;
		}

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) { }
		public void Use() { }
		public void Unuse() { }
	}
}