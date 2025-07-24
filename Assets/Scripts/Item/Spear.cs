using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Spear : MonoBehaviour, IUsableItem {
		private bool _attacking;
		private InputSystem_Actions _ia;

		private void Start() {
			_ia = new InputSystem_Actions();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Spear.Attack.performed += OnAttack;
		}

		private void OnDisable() {
			_ia.Spear.Attack.performed -= OnAttack;
			_ia.Disable();
		}

		private void OnTriggerEnter(Collider other) {
			if (!_attacking || !other.TryGetComponent(out SharkAi shark)) return;
			shark.TakeShot(transform.position);
		}

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) {
		}

		public void OnAnimationEnd() {
			_attacking = false;
		}

		private void OnAttack(InputAction.CallbackContext _) {
			_attacking = true;
		}
	}
}