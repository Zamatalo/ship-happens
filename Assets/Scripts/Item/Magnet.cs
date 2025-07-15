using Player;
using UnityEngine;

namespace Item {
	public class Magnet : MonoBehaviour {
		public GameObject playerObj;
		public Transform origin;
		public float range = 100f;
		public float pullSpeed = 10f;
		public float collectDist = 1f;
		private PlayerInventory _playerInventory;

		private void Start() {
			_playerInventory = playerObj.GetComponent<PlayerInventory>();
		}

		private void FixedUpdate() {
			AttractObjects();
		}

		private void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawRay(origin.position, origin.forward * range);
		}

		private void AttractObjects() {
			if (!Physics.Raycast(origin.position, origin.forward, out var hit, range)) return;
			if (!hit.collider.TryGetComponent(out Loot.Loot loot)) return;

			loot.transform.position = Vector3.MoveTowards(loot.transform.position, origin.position,
				pullSpeed * Time.deltaTime);

			if (Vector3.Distance(loot.transform.position, origin.position) < collectDist) {
				_playerInventory.AddMetal(loot.amount);
				Destroy(hit.collider.gameObject);
			}
		}
	}
}