using Player;
using Player.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Edible : MonoBehaviour {
		private InputSystem_Actions _ia;
		private PlayerInventory _inventory;
		private PlayerNeeds _needs;
		private Slot _slot;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Potable.Drink.performed += OnEat;
		}

		private void OnDisable() {
			_ia.Potable.Drink.performed -= OnEat;
			_ia.Disable();
		}

		public void Init(PlayerNeeds needs, PlayerInventory inventory, Slot slot) {
			_needs = needs;
			_inventory = inventory;
			_slot = slot;
		}

		private void OnEat(InputAction.CallbackContext _) {
			_inventory.Consume(1);

			_needs.hunger = Mathf.Min(_needs.maxHunger, _needs.hunger + 70);
		}
	}
}