using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Potable : MonoBehaviour {
		private float _amount;
		private InputSystem_Actions _ia;
		private PlayerNeeds _playerNeeds;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Potable.Drink.performed += OnDrink;
		}

		private void OnDisable() {
			_ia.Potable.Drink.performed -= OnDrink;
			_ia.Disable();
		}

		public void Init(PlayerNeeds playerNeeds, float amount) {
			_playerNeeds = playerNeeds;
			_amount = amount;
		}

		private void OnDrink(InputAction.CallbackContext _) {
			_playerNeeds.thirst = Mathf.Min(_playerNeeds.maxThirst, _playerNeeds.thirst + _amount);
		}
	}
}