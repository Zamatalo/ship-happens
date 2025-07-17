using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class Magnet : MonoBehaviour {
		public float pullSpeed = 10f;
		public float maxDist = 100f;
		public AudioSource magnetSound;
		private InputSystem_Actions _ia;
		private Transform _origin;
		private bool _pulling;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void FixedUpdate() {
			if (_pulling) Pull();
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.Magnet.Pull.performed += OnPullStart;
			_ia.Magnet.Pull.canceled += OnPullEnd;
		}

		private void OnDisable() {
			_ia.Magnet.Pull.performed -= OnPullStart;
			_ia.Magnet.Pull.canceled -= OnPullEnd;
			_ia.Disable();
		}

		public void Init(Transform origin) {
			_origin = origin;
		}

		private void OnPullStart(InputAction.CallbackContext _) {
			_pulling = true;
			magnetSound.Play();
		}

		private void OnPullEnd(InputAction.CallbackContext _) {
			magnetSound.Stop();
			_pulling = false;
		}

		private void Pull() {
			if (!Physics.Raycast(_origin.position, _origin.forward, out var hit, maxDist)) return;
			if (!hit.collider.TryGetComponent(out Loot.Loot loot)) return;

			loot.transform.position = Vector3.MoveTowards(
				loot.transform.position,
				_origin.position,
				pullSpeed * Time.fixedDeltaTime
			);
		}
	}
}