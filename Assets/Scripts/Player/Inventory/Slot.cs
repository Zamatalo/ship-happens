// using TMPro;
// using UnityEngine;
//
// namespace Player.Inventory {
//     public class Slot : MonoBehaviour {
//         
//         private Sprite BackgroundIcon{get;set;}
//         public Sprite ItemIcon {get;set;} = null; 
//         private TMP_Text _amountText;
//         public ItemEnums Kind { get; private set; } = ItemEnums.None;
//         public int Amount { get; private set; } = 0;
//
//         public void Awake()
//         {
//             SetBackgroundIconInactive();
//         }
//     
//         public void SetSlot(ItemEnums kind, int initialAmount = 1) {
//             Kind = kind;
//             Amount = initialAmount;
//             UpdateAmountText();
//         }
//
//         public void AddAmount(int value) {
//             Amount += value;
//             UpdateAmountText();
//         }
//
//         public void SetAmount(int value) {
//             Amount = value;
//             UpdateAmountText();
//         }
//
//         public void SetBackgroundIconActive()
//         {
//             BackgroundIcon = Resources.Load<Sprite>("Sprites/ItemEnable");
//         }
//         public void SetBackgroundIconInactive()
//         {
//             BackgroundIcon = Resources.Load<Sprite>("Sprites/ItemDisable");            
//         }
//         private void UpdateAmountText() {
//             if (_amountText != null) {
//                 _amountText.text = Amount.ToString();
//             }
//         }
//
//         public bool IsStackable() {
//             return Kind switch {
//                 ItemEnums.GravityGun => false,
//                 ItemEnums.Harpoon => false,
//                 ItemEnums.Magnet => false,
//                 _ => true
//             };
//         }
//     }
// }
