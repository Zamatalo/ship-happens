using System;
using TMPro;
using UnityEngine;

namespace Player.Inventory {
    public class Slot : MonoBehaviour {
        

        private TMP_Text _amountText;
        public SlotKind Kind { get; private set; } = SlotKind.None;
        public int Amount { get; private set; } = 0;

        public void SetSlot(SlotKind kind, int initialAmount = 1) {
            Kind = kind;
            Amount = initialAmount;
            UpdateAmountText();
        }

        public void AddAmount(int value) {
            Amount += value;
            UpdateAmountText();
        }

        public void SetAmount(int value) {
            Amount = value;
            UpdateAmountText();
        }

        private void UpdateAmountText() {
            if (_amountText != null) {
                _amountText.text = Amount.ToString();
            }
        }

        public bool IsStackable() {
            return Kind switch {
                SlotKind.GravityGun => false,
                SlotKind.Metal => false,
                _ => true
            };
        }
    }
}
public enum SlotKind {
    None,
    Magnet,
    Harpoon,
    Metal,
    GravityGun
}