using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player.Inventory {
	public class PlayerInventory : MonoBehaviour {
		public delegate void OnSlotChange(Slot slot);
		public List<GameObject> slots;
		public Sprite activeSprite;
		public Sprite inactiveSprite;
		private readonly List<OnSlotChange> _fns = new();
		private readonly List<Slot> _slots = new();
		private byte _activeSlotIndex;
		private InputSystem_Actions _inputActions;

		private void Awake() {
			_inputActions = new InputSystem_Actions();
		}

		private void Start() {
			foreach (var slot in slots) {
				slot.GetComponent<Image>().color = inactiveColor;
				_slots.Add(slot.GetComponent<Slot>());
			}
			slots[_activeSlotIndex].GetComponent<Image>().color = activeColor;
		}

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
			OnActiveSlotChange((byte)((_activeSlotIndex - 1 + slots.Count) % slots.Count));
		}

		private void NextSlot() {
			OnActiveSlotChange((byte)((_activeSlotIndex + 1) % slots.Count));
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
			slots[_activeSlotIndex].GetComponent<Image>().color = inactiveColor;
			_activeSlotIndex = index;
			slots[_activeSlotIndex].GetComponent<Image>().color = activeColor;

			foreach (var fn in _fns) fn.Invoke(_slots[_activeSlotIndex]);
		}

		public bool Collect(Slot.SlotKind slotKind, int amount) {
			foreach (var slot in _slots) {
				if (slot.kind == Slot.SlotKind.None) {
					slot.kind = slotKind;
					slot.amount = amount;
					return true;
				}

				if (slot.kind == slotKind && slot.Stackable()) {
					slot.amount += amount;
					return true;
				}
			}

			return false;
		}
	}
}