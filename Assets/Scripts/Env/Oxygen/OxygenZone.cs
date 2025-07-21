using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Oxygen {
	public class OxygenZone : MonoBehaviour {
		public float oxygenGenerationSpeed = 1f;
		private readonly List<PlayerNeeds> _playerNeedss = new();

		private void FixedUpdate() {
			foreach (var playerNeeds in _playerNeedss)
				playerNeeds.oxygen = Mathf.Min(
					playerNeeds.maxOxygen,
					playerNeeds.oxygen + oxygenGenerationSpeed * Time.fixedDeltaTime
				);
		}

		private void OnTriggerEnter(Collider other) {
			if (other.TryGetComponent(out PlayerNeeds playerNeeds)) _playerNeedss.Add(playerNeeds);
		}

		private void OnTriggerExit(Collider other) {
			if (other.TryGetComponent(out PlayerNeeds playerNeeds))
				_playerNeedss.Remove(playerNeeds);
		}
	}
}