using Projectile;
using Unity.VisualScripting;
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
		private bool _pulling;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void FixedUpdate() {
			if (ReferenceEquals(_currentHarpoon, null) || _currentHarpoon.IsDestroyed()) {
				harpoonObj.SetActive(true);
				_pulling = false;
				return;
			}

			if (_pulling) _currentHarpoon.Pull();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Harpoon.Shoot.performed += OnShoot;
			_ia.Harpoon.Pull.performed += OnPullStart;
			_ia.Harpoon.Pull.canceled += OnPullEnd;
			_ia.Harpoon.Cancel.performed += OnCancel;
		}

		private void OnDisable() {
			_ia.Harpoon.Shoot.performed -= OnShoot;
			_ia.Harpoon.Pull.performed -= OnPullStart;
			_ia.Harpoon.Pull.canceled -= OnPullEnd;
			_ia.Harpoon.Cancel.performed -= OnCancel;
			_ia.Disable();
		}

		private void OnDestroy() {
			if (_currentHarpoon != null) Destroy(_currentHarpoon.gameObject);
		}

		public void Init(Transform origin) {
			_origin = origin;
		}

		private void OnShoot(InputAction.CallbackContext _) {
			if (_currentHarpoon != null) return;
			FireHarpoon();
		}

		private void OnPullStart(InputAction.CallbackContext _) {
			if (_currentHarpoon == null || !_currentHarpoon.hit) return;
			_pulling = true;
		}

		private void OnPullEnd(InputAction.CallbackContext _) {
			if (_currentHarpoon == null || !_currentHarpoon.hit) return;
			_pulling = false;
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
				_origin.position,
				Quaternion.LookRotation(_origin.forward) * Quaternion.Euler(90f, 0f, 0f)
			);

			_currentHarpoon = inst.GetComponent<Harpoon>();
			_currentHarpoon.Init(notActualOrigin, _origin, shootForce);

			shootSound.Play();
			harpoonObj.SetActive(false);
		}
	}
}