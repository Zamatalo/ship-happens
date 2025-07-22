using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Edible : MonoBehaviour {
		private InputSystem_Actions _ia;
		private PlayerInventoryNew _inventory;
		private PlayerNeeds _needs;
		private int amount;
		private string kind;

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

		public void Init(PlayerNeeds needs, PlayerInventoryNew inventory, string kind, int amount) {
			_needs = needs;
			_inventory = inventory;
			kind = kind;
			amount = amount;
		}

		private void OnEat(InputAction.CallbackContext _) {
			//_inventory.Consume(1);

			_needs.hunger = Mathf.Min(_needs.maxHunger, _needs.hunger + 70);
		}
	}
}