using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player {
	public class HealthBar : MonoBehaviour {
		public GameObject playerObject;
		private PlayerNeeds _playerNeeds;
		private Slider _slider;

		private void Start() {
			_playerNeeds = playerObject.GetComponent<PlayerNeeds>();
			_slider = GetComponent<Slider>();

			_slider.maxValue = _playerNeeds.maxHealth;
			_slider.value = _playerNeeds.health;
		}

		private void Update() {
			_slider.value = _playerNeeds.health;
		}
	}
}