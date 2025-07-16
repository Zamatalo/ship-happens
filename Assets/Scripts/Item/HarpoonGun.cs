using Projectile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
    public class HarpoonGun : MonoBehaviour {
        public GameObject harpoonObj;
        public Transform origin;
        public Transform notActualOrigin;
        public GameObject harpoonPrefab;
        public float shootForce = 50f;

        private InputSystem_Actions _ia;
        private Harpoon _currentHarpoon;

        private void Awake() {
            _ia = new InputSystem_Actions();
        }

        private void OnEnable() {
            _ia.Enable();
            _ia.Harpoon.Shoot.performed += OnShoot;
            _ia.Harpoon.Cancel.performed += OnCancel;
        }

        private void OnDisable() {
            _ia.Harpoon.Shoot.performed -= OnShoot;
            _ia.Harpoon.Cancel.performed -= OnCancel;
            _ia.Disable();
        }

        private void OnShoot(InputAction.CallbackContext _) {
            if (_currentHarpoon != null) return;

            FireHarpoon();
        }

        private void OnCancel(InputAction.CallbackContext _) {
            if (_currentHarpoon != null) {
                Destroy(_currentHarpoon.gameObject);
                _currentHarpoon = null;
                harpoonObj.SetActive(true);
            }
        }
        
        private void FireHarpoon() {
            var inst = Instantiate(
                harpoonPrefab,
                notActualOrigin.position,
                Quaternion.LookRotation(origin.forward) * Quaternion.Euler(90f, 0f, 0f)
            );

            _currentHarpoon = inst.GetComponent<Harpoon>();
            _currentHarpoon.Init(origin.forward, shootForce, notActualOrigin);

            harpoonObj.SetActive(false);
        }
    }
}