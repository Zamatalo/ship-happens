using Player;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Potable : MonoBehaviour {
		private InputSystem_Actions _ia;
		private PlayerInventory _inventory;
		private PlayerNeeds _needs;
		private Slot _slot;

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

		public void Init(PlayerNeeds needs, PlayerInventory inventory, Slot slot) {
			_needs = needs;
			_inventory = inventory;
			_slot = slot;
		}

		private void OnDrink(InputAction.CallbackContext _) {
			_inventory.Consume(1);

			_needs.thirst = Mathf.Min(_needs.maxThirst, _needs.thirst + 70);
		}
	}
}