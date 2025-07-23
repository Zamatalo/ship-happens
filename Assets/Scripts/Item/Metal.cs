using System.Collections.Generic;
using UnityEngine;

namespace Item {
	public class Metal : MonoBehaviour, IUsableItem {
		public string Name() {
			return "Metal";
		}

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) { }
		public void Use() { }
		public void Unuse() { }
	}
}