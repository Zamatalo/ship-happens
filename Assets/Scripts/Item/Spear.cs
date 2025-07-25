using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Spear : MonoBehaviour, IUsableItem {
		private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
		public bool attacking;
		public bool hasAttackedShark;
		private Animator _anim;
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
			if (!attacking || !other.TryGetComponent(out SharkAi shark)) return;
			shark.TakeShot(transform.position);
		}

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) {
		}

		public void OnAnimationEnd() {
			if (!attacking) return;
			_anim.SetBool(IsAttacking, false);
			attacking = false;
			hasAttackedShark = false;
		}

		private void OnAttack(InputAction.CallbackContext _) {
			if (attacking) return;
			_anim.SetBool(IsAttacking, true);
			attacking = true;
		}
	}
}