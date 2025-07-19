using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player.Inventory {
	public class PlayerInventory : MonoBehaviour {
		public delegate void OnDropItem(Slot slot);
		public delegate void OnSlotChange(Slot slot);
		public List<GameObject> slots;
		public Color activeColor;
		public Color inactiveColor;
		private readonly List<OnDropItem> _dropItemListeners = new();
		private readonly List<OnSlotChange> _slotChangeListeners = new();
		private readonly List<Slot> _slots = new();
		private byte _i;
		private InputSystem_Actions _ia;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			foreach (var slot in slots) {
				slot.GetComponent<Image>().color = inactiveColor;
				_slots.Add(slot.GetComponent<Slot>());
			}
			slots[_i].GetComponent<Image>().color = activeColor;
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Inventory._1.performed += _1;
			_ia.Inventory._2.performed += _2;
			_ia.Inventory._3.performed += _3;
			_ia.Inventory._4.performed += _4;
			_ia.Inventory._5.performed += _5;
			_ia.Inventory.IterMouse.performed += IterMouse;
			_ia.Inventory.IterKeyboard.performed += IterKeyboard;
			_ia.Inventory.DropItem.performed += DropItem;
		}

		private void OnDisable() {
			_ia.Inventory._1.performed -= _1;
			_ia.Inventory._2.performed -= _2;
			_ia.Inventory._3.performed -= _3;
			_ia.Inventory._4.performed -= _4;
			_ia.Inventory._5.performed -= _5;
			_ia.Inventory.IterMouse.performed -= IterMouse;
			_ia.Inventory.IterKeyboard.performed -= IterKeyboard;
			_ia.Inventory.DropItem.performed -= DropItem;
			_ia.Disable();
		}

		public void AddSlotChangeListener(OnSlotChange fn) {
			_slotChangeListeners.Add(fn);
		}

		public void AddDropItemListener(OnDropItem fn) {
			_dropItemListeners.Add(fn);
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

		private void DropItem(InputAction.CallbackContext _) {
			if (_slots[_i].kind == Slot.SlotKind.None) return;

			foreach (var fn in _dropItemListeners) fn.Invoke(_slots[_i]);

			_slots[_i].kind = Slot.SlotKind.None;
			_slots[_i].amount = 0;

			OnActiveSlotChange(_i);
		}

		private void PrevSlot() {
			OnActiveSlotChange((byte)((_i - 1 + slots.Count) % slots.Count));
		}

		private void NextSlot() {
			OnActiveSlotChange((byte)((_i + 1) % slots.Count));
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
			slots[_i].GetComponent<Image>().color = inactiveColor;
			_i = index;
			slots[_i].GetComponent<Image>().color = activeColor;

			foreach (var fn in _slotChangeListeners) fn.Invoke(_slots[_i]);
		}

		public bool Collect(Slot.SlotKind slotKind, int amount, float? jetpackFuel) {
			foreach (var slot in _slots) {
				if (slot.kind == Slot.SlotKind.None) {
					slot.kind = slotKind;
					slot.amount = amount;
					slot.JetpackFuel = jetpackFuel;
					return true;
				}

				if (slot.kind == slotKind && slot.Stackable()) {
					slot.amount += amount;
					return true;
				}
			}

			return false;
		}

		public void Consume(int amount) {
			_slots[_i].amount = Mathf.Max(0, _slots[_i].amount - amount);
			if (_slots[_i].amount > 0) return;

			_slots[_i].kind = Slot.SlotKind.None;
			OnActiveSlotChange(_i);
		}
	}
}