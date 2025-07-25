using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Spear : MonoBehaviour, IUsableItem {
		private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
		private Animator _anim;
		private bool _attacking;
		private InputSystem_Actions _ia;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			_anim = GetComponent<Animator>();
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
			if (!_attacking) return;
			_anim.SetBool(IsAttacking, false);
			_attacking = false;
		}

		private void OnAttack(InputAction.CallbackContext _) {
			if (_attacking) return;
			_anim.SetBool(IsAttacking, true);
			_attacking = true;
		}
	}
}