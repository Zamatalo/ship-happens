using Projectile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class HarpoonGun : MonoBehaviour {
		public AudioSource shootSound;
		public GameObject harpoonObj;
		public Transform notActualOrigin;
		public GameObject harpoonPrefab;
		public float shootForce = 50f;
		private Harpoon _currentHarpoon;
		private InputSystem_Actions _ia;
		private Transform _origin;

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

		public void Init(Transform origin) {
			_origin = origin;
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
				Quaternion.LookRotation(_origin.forward) * Quaternion.Euler(90f, 0f, 0f)
			);

			_currentHarpoon = inst.GetComponent<Harpoon>();
			_currentHarpoon.Init(_origin.forward, shootForce);

			shootSound.Play();
			harpoonObj.SetActive(false);
		}
	}
}