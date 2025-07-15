using UnityEngine;

namespace Inventory {
    public class Slot : MonoBehaviour {
        public SlotKind kind;
        
        private void Start() {
        
        }

        private void Update() {
        
        }

        public enum SlotKind {
            NONE,
            HOOK,
        }
    }
}
