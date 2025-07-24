using System;
using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Potable : MonoBehaviour, IUsableItem {
		private PlayerInventoryNew _inventory;
		private PlayerNeeds _needs;

		private InputSystem_Actions _ia;
		
		
		// public override void OnNetworkSpawn()
		// {
		// 	if (_inventory == null)
		// 	{
		// 		Transform playerTransform = transform.parent;
		// 		if (playerTransform != null)
		// 		{
		// 			_inventory = playerTransform.Find("Inventory")?.GetComponent<PlayerInventoryNew>();
		// 		}
		// 	}
		// 	
		// 	if (_inventory == null)
		// 	{
		// 		Debug.LogError("Inventory reference not found!", this);
		// 	}
		// }
		
		public void Init(Transform origin, List<ItemParameter> parameters) {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerInventoryNew>();
			_needs = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponent<PlayerNeeds>();
		}

		public void Use() {
			_needs.thirst = Mathf.Min(_needs.maxThirst, _needs.thirst + _needs.maxThirst / 3f);
			_inventory.RemoveItemServerRpc("Potable", 1);
		}
		
		
		public void OnEnable()
		{
			_ia = new  InputSystem_Actions();
			_ia.Player.Enable();
			_ia.Player.UseItem.performed += OnUsePerformed;
		}

		public void OnDisable()
		{
			_ia.Player.UseItem.performed -= OnUsePerformed;
			_ia.Player.Disable();
		}

		private void OnUsePerformed(InputAction.CallbackContext context)
		{
			Use();
		}
		private void OnDestroy()
		{
			OnDisable();
		}
	}
}