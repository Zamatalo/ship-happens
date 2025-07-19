using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player.Needs {
	public class ThirstBar : MonoBehaviour {
		public GameObject playerObject;
		private PlayerNeeds _playerNeeds;
		private Slider _slider;

		private void Start() {
			_playerNeeds = playerObject.GetComponent<PlayerNeeds>();
			_slider = GetComponent<Slider>();

			_slider.maxValue = _playerNeeds.maxThirst;
			_slider.value = _playerNeeds.thirst;
		}

		private void Update() {
			_slider.value = _playerNeeds.thirst;
		}
	}
}