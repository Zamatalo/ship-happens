using System.Collections.Generic;
using Projectile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class HarpoonGun : MonoBehaviour, IUsableItem {
		[Header("References")] [SerializeField]
		private GameObject harpoonObj;
		[SerializeField] private Transform notActualOrigin;
		[SerializeField] private GameObject harpoonPrefab;
		[SerializeField] private GameObject nakonechnik;

		[Header("Settings")] [SerializeField] private float shootForce = 50f;
		[SerializeField] private float pullSpeed = 5f;
		[SerializeField] private Vector3 ugolNakonechnik;

		[Header("Animator")] [SerializeField] private Animator animator;
		private Harpoon _currentHarpoon;
		
		private InputSystem_Actions _ia;
		
		private bool _isBusy;
		private Transform _origin;
		private void Awake() {
			_ia = new InputSystem_Actions();
		}
		private void OnDestroy() {
			if (_currentHarpoon != null) {
				Destroy(_currentHarpoon.gameObject);
			}
		}

		public void Init(Transform origin, List<ItemParameter> itemParameters) {
			_origin = origin;
		}

		public void Use(InputAction.CallbackContext context) {
			if (_isBusy) return;
			if (_currentHarpoon != null && _currentHarpoon.hit) {
				nakonechnik.SetActive(false);
				_currentHarpoon.StartPull();
				return;
			}

			if (_currentHarpoon == null) {
				nakonechnik.SetActive(true);
				FireHarpoon();
			}
		}

		public void Unuse(InputAction.CallbackContext context) {
			if (_isBusy) return;
			if (_currentHarpoon != null) {
				_currentHarpoon.StopPull();

				if (!_currentHarpoon.hit) {
					nakonechnik.SetActive(true);
					Destroy(_currentHarpoon.gameObject);
					_currentHarpoon = null;
					if (harpoonObj != null) harpoonObj.SetActive(true);
					Reload();
				}
			}
		}

		private void FireHarpoon() {
			if (_isBusy) return;
			if (harpoonPrefab == null) return;
			if (harpoonPrefab.GetComponent<Harpoon>() == null) return;
			if (notActualOrigin == null || _origin == null) return;

			var inst = Instantiate(
				harpoonPrefab,
				_origin.position,
				Quaternion.LookRotation(_origin.forward) * Quaternion.Euler(ugolNakonechnik)
			);
			nakonechnik.SetActive(false);
			animator.SetTrigger("Shoot");

			_currentHarpoon = inst.GetComponent<Harpoon>();
			_currentHarpoon.Init(notActualOrigin, _origin, shootForce);
			_currentHarpoon.pullSpeed = pullSpeed;
		}

		public void Reload() {
			if (_isBusy) return;
			_isBusy = true;

			if (_currentHarpoon != null) {
				Destroy(_currentHarpoon.gameObject);
				_currentHarpoon = null;
			}

			if (harpoonObj != null) {
				harpoonObj.SetActive(true);
			}

			if (nakonechnik != null) {
				nakonechnik.SetActive(true);
			}

			animator.SetTrigger("Reload");
		}

		public void OnAnimationEnd() {
			_isBusy = false;
		}

		public void OnEnable()
		{
			_ia.Player.Enable();
			_ia.Player.UseItem.started += Use;
		}

		public void OnDisable()
		{
			_ia.Player.UseItem.canceled -= Unuse;
			_ia.Player.Disable();
		}


	}
}