using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Edible : MonoBehaviour {
		private float _amount;
		private InputSystem_Actions _ia;
		private PlayerNeeds _playerNeeds;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Edible.Eat.performed += OnEat;
		}

		private void OnDisable() {
			_ia.Edible.Eat.performed -= OnEat;
			_ia.Disable();
		}

		public void Init(PlayerNeeds playerNeeds, float amount) {
			_playerNeeds = playerNeeds;
			_amount = amount;
		}

		private void OnEat(InputAction.CallbackContext _) {
			_playerNeeds.hunger = Mathf.Min(_playerNeeds.maxHunger, _playerNeeds.hunger + _amount);
		}
	}
}