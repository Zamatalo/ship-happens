using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Potable : MonoBehaviour {
		private InputSystem_Actions _ia;
		private PlayerInventoryNew _inventory;
		private PlayerNeeds _needs;
		private int _amount;
		private string _kind;

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

		// public void Init(PlayerNeeds needs, PlayerInventoryNew inventory, string kind, int amount) {
		// 	_needs = needs;
		// 	_inventory = inventory;
		// 	_kind = kind;
		// 	_amount = amount;
		// }

		private void OnDrink(InputAction.CallbackContext _) {
			//_inventory.Consume(1);

			_needs.thirst = Mathf.Min(_needs.maxThirst, _needs.thirst + 70);
		}
	}
}