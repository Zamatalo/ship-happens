using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Player.Inventory {
	public class PlayerInventoryHotbar : MonoBehaviour {
		public delegate void OnSlotChange(InventoryItem item);
		public List<InventoryItem> InventoryItems = new List<InventoryItem>();
		private readonly List<OnSlotChange> _fns = new();
		private byte _activeSlotIndex;
		private InputSystem_Actions _inputActions;
		private VisualElement m_Root;
		private VisualElement m_SlotContainer;
		
		private void Awake() {
			_inputActions = new InputSystem_Actions();
			
			m_Root = GetComponent<UIDocument>().rootVisualElement;
			m_SlotContainer = m_Root.Query("slotsHotbar").First();
	
			for (int i = 0; i < 5; i++)
			{
				InventoryItem item = new InventoryItem();

				InventoryItems.Add(item);
				m_SlotContainer.Add(item);
			}
		}

		// private void Start() {
		// 	foreach (var slot in slots) {
		// 		slot.GetComponent<Image>().color = inactiveColor;
		// 		_slots.Add(slot.GetComponent<Slot>());
		// 	}
		// 	InventoryItems[_activeSlotIndex];
		// }

		private void OnEnable() {
			_inputActions.Enable();
			_inputActions.Inventory._1.performed += _1;
			_inputActions.Inventory._2.performed += _2;
			_inputActions.Inventory._3.performed += _3;
			_inputActions.Inventory._4.performed += _4;
			_inputActions.Inventory._5.performed += _5;
			_inputActions.Inventory.IterMouse.performed += IterMouse;
			_inputActions.Inventory.IterKeyboard.performed += IterKeyboard;
		}

		private void OnDisable() {
			_inputActions.Inventory._1.performed -= _1;
			_inputActions.Inventory._2.performed -= _2;
			_inputActions.Inventory._3.performed -= _3;
			_inputActions.Inventory._4.performed -= _4;
			_inputActions.Inventory._5.performed -= _5;
			_inputActions.Inventory.IterMouse.performed -= IterMouse;
			_inputActions.Inventory.IterKeyboard.performed -= IterKeyboard;
			_inputActions.Disable();
		}

		public void AddSlotChangeListener(OnSlotChange fn) {
			_fns.Add(fn);
		}

		private void IterMouse(InputAction.CallbackContext ctx) {
			var dir = ctx.ReadValue<Vector2>();
			if (dir.y > 0)
				PrevSlot();
			else if (dir.y < 0) NextSlot();
		}

		private void IterKeyboard(InputAction.CallbackContext ctx) {
			if (Keyboard.current.shiftKey.isPressed)
				PrevSlot();
			else
				NextSlot();
		}

		private void PrevSlot() {
			OnActiveSlotChange((byte)((_activeSlotIndex - 1 + InventoryItems.Count) % InventoryItems.Count));
		}

		private void NextSlot() {
			OnActiveSlotChange((byte)((_activeSlotIndex + 1) % InventoryItems.Count));
		}

		private void _1(InputAction.CallbackContext _) {
			OnActiveSlotChange(0);
		}

		private void _2(InputAction.CallbackContext _) {
			OnActiveSlotChange(1);
		}

		private void _3(InputAction.CallbackContext _) {
			OnActiveSlotChange(2);
		}

		private void _4(InputAction.CallbackContext _) {
			OnActiveSlotChange(3);
		}

		private void _5(InputAction.CallbackContext _) {
			OnActiveSlotChange(4);
		}

		private void OnActiveSlotChange(byte index) {
			InventoryItems[_activeSlotIndex].SetActive(true);
			_activeSlotIndex = index;
		}

		// public void Collect(InventoryItem itemNew, int amount) {
		// 	foreach (var item in InventoryItems)
		// 	{
		// 		//already in inv
		// 		if (item.Kind.Equals(itemNew.Kind) && !itemNew.Kind.Equals(ItemEnums.None))
		// 		{
		// 			
		// 		}	
		// 	}
		// 	// foreach (var item in InventoryItems) {
		// 	// 	if (itemNew.Kind == ItemEnums.None) {
		// 	// 		item.kind = slotKind;
		// 	// 		slot.amount = amount;
		// 	// 		return true;
		// 	// 	}
		// 	//
		// 	// 	if (slot.kind == slotKind && slot.Stackable()) {
		// 	// 		slot.amount += amount;
		// 	// 		return true;
		// 	// 	}
		// 	// }
		// 	//
		// 	// return false;
		// }
	}
}