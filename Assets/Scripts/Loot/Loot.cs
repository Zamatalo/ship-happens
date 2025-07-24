using UnityEngine;

namespace Loot {
	public class Loot : MonoBehaviour {
		public InventoryItem kind;
		private float _maxVelX; // Maximum speed of falling
		private float _maxVelZ; // Maximum speed of falling
		private float _minVelX; // Minimum speed of falling
		private float _minVelZ; // Minimum speed of falling
		private Rigidbody _rb;
		private float _rotSpeed;

		private void Start() {
			_rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the loot object
			var x = Random.Range(_minVelX, _maxVelX);
			var z = Random.Range(_minVelZ, _maxVelZ);
			_rb.linearVelocity = new Vector3(x, 0, z); // Move downwards in the negative Z direction

			var randomX = Random.Range(-1f, 1f) * _rotSpeed;
			var randomY = Random.Range(-1f, 1f) * _rotSpeed;
			var randomZ = Random.Range(-1f, 1f) * _rotSpeed;
			_rb.angularVelocity = new Vector3(randomX, randomY, randomZ);

			Destroy(gameObject, 100);
		}

		public void Init(float minVelX, float maxVelX, float minVelZ, float maxVelZ,
			float rotSpeed) {
			_minVelX = minVelX;
			_maxVelX = maxVelX;
			_minVelZ = minVelZ;
			_maxVelZ = maxVelZ;
			_rotSpeed = rotSpeed;
		}
	}
}