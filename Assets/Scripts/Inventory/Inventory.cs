using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Inventory {
    public class Inventory : MonoBehaviour {
        public List<GameObject> slotObjects;
        public Color activeColor;
        public Color inactiveColor;
        private readonly List<Slot> _slots = new();
        private readonly List<OnSlotChange> _fns = new();
        private InputSystem_Actions _inputActions;
        private byte _activeSlotIndex;

        private void Awake() {
            _inputActions = new InputSystem_Actions();
        }
    
        private void Start() {
            foreach (var slot in slotObjects) {
                slot.GetComponent<Image>().color = inactiveColor;
                _slots.Add(slot.GetComponent<Slot>());
            }
            slotObjects[_activeSlotIndex].GetComponent<Image>().color = activeColor;
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
                if (dir.y > 0) {
                    PrevSlot();
                } else if (dir.y < 0) {
                    NextSlot();
                }
        }

        private void IterKeyboard(InputAction.CallbackContext ctx) {
            if (Keyboard.current.shiftKey.isPressed) {
                PrevSlot();
            } else {
                NextSlot();
            }
        }

        private void PrevSlot() {
            OnActiveSlotChange((byte) ((_activeSlotIndex - 1 + slotObjects.Count) % slotObjects.Count));
        }

        private void NextSlot() {
            OnActiveSlotChange((byte) ((_activeSlotIndex + 1) % slotObjects.Count));
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
            slotObjects[_activeSlotIndex].GetComponent<Image>().color = inactiveColor;
            _activeSlotIndex = index;
            slotObjects[_activeSlotIndex].GetComponent<Image>().color = activeColor;

            foreach (var fn in _fns) {
                fn.Invoke(_slots[_activeSlotIndex]);
            }
        }
        
        public delegate void OnSlotChange(Slot slot);
    }
}
