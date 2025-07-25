using System.Collections.Generic;
using UnityEngine;

namespace Item {
	public class EmptyItem : MonoBehaviour, IUsableItem {
		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) { }
	}
}