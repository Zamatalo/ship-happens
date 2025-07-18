using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player {
	public class OxygenBar : MonoBehaviour {
		public GameObject playerObject;
		private PlayerNeeds _playerNeeds;
		private Slider _slider;

		private void Start() {
			_playerNeeds = playerObject.GetComponent<PlayerNeeds>();
			_slider = GetComponent<Slider>();

			_slider.maxValue = _playerNeeds.maxOxygen;
			_slider.value = _playerNeeds.oxygen;
		}

		private void Update() {
			_slider.value = _playerNeeds.oxygen;
		}
	}
}