using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player.Needs {
	public class HungerBar : MonoBehaviour {
		public GameObject playerObject;
		private PlayerNeeds _playerNeeds;
		private Slider _slider;

		private void Start() {
			_playerNeeds = playerObject.GetComponent<PlayerNeeds>();
			_slider = GetComponent<Slider>();

			_slider.maxValue = _playerNeeds.maxHunger;
			_slider.value = _playerNeeds.hunger;
		}

		private void Update() {
			_slider.value = _playerNeeds.hunger;
		}
	}
}