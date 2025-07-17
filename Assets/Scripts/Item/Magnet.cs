// using Player.Inventory;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// namespace Item {
// 	public class Magnet : MonoBehaviour {
// 		public GameObject player;
// 		public Transform origin;
// 		public float pullSpeed = 10f;
// 		public float maxDist = 100f;
// 		public float minDist = 1f;
// 		private InputSystem_Actions _ia;
// 		private PlayerInventory _playerInventory;
// 		private bool _pulling;
//
// 		private void Awake() {
// 			_ia = new InputSystem_Actions();
// 		}
//
// 		private void Start() {
// 			_playerInventory = player.GetComponent<PlayerInventory>();
// 		}
//
// 		private void FixedUpdate() {
// 			if (_pulling) Pull();
// 		}
//
// 		private void OnEnable() {
// 			_ia.Enable();
// 			_ia.Magnet.Pull.performed += OnPullStart;
// 			_ia.Magnet.Pull.canceled += OnPullEnd;
// 		}
//
// 		private void OnDisable() {
// 			_ia.Magnet.Pull.performed -= OnPullStart;
// 			_ia.Magnet.Pull.canceled -= OnPullEnd;
// 			_ia.Disable();
// 		}
//
// 		private void OnPullStart(InputAction.CallbackContext _) {
// 			_pulling = true;
// 		}
//
// 		private void OnPullEnd(InputAction.CallbackContext _) {
// 			_pulling = false;
// 		}
//
// 		private void Pull() {
// 			if (!Physics.Raycast(origin.position, origin.forward, out var hit, maxDist)) return;
// 			if (!hit.collider.TryGetComponent(out Loot.Loot loot)) return;
//
// 			loot.transform.position = Vector3.MoveTowards(
// 				loot.transform.position,
// 				origin.position,
// 				pullSpeed * Time.fixedDeltaTime
// 			);
//
// 			if (Vector3.Distance(loot.transform.position, origin.position) > minDist) return;
//
// 			if (_playerInventory.Collect(loot.kind, loot.amount)) Destroy(hit.collider.gameObject);
// 		}
// 	}
// }