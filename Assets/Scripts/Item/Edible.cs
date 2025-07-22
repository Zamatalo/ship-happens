using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Edible : MonoBehaviour,IUsableItem {
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
			_ia.Potable.Drink.performed += OnEat;
		}

		private void OnDisable() {
			_ia.Potable.Drink.performed -= OnEat;
			_ia.Disable();
		}

		public void Init(PlayerNeeds needs, PlayerInventoryNew inventory, string kind, int amount) {
			
		}

		private void OnEat(InputAction.CallbackContext _) {
			//_inventory.Consume(1);

			_needs.hunger = Mathf.Min(_needs.maxHunger, _needs.hunger + 70);
		}

		public void Init(Transform origin, List<ItemParameter> parameters)
		{
			throw new System.NotImplementedException();
		}

		// public void Init(Transform origin, List<ItemParameter> itemParameters)
		// {
		// 	_needs = itemParameters.Find(e=>e.key.Equals("Needs")).value;
  //        	_inventory = inventory;
  //        	_kind = kind;
  //        	_amount = amount;
		// }

		public void Use()
		{
			throw new System.NotImplementedException();
		}

		public void Unuse()
		{
			throw new System.NotImplementedException();
		}
	}
}