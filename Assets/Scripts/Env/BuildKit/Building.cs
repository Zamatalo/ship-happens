using UnityEngine;

namespace BuildKit {
	public class Building : MonoBehaviour {
		public int maxHealth;
		private int _health;

		private void Awake() {
			_health = maxHealth;
		}

		public bool TakeDamage(int damage) {
			if (_health > damage) {
				_health -= damage;
				return false;
			}

			Destroy(gameObject);
			return true;
		}
	}
}