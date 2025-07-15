using UnityEngine;

namespace Player {
	public class PlayerInventory : MonoBehaviour {
		private int _metal;
		private int _paper;

		public int Metal() {
			return _metal;
		}

		public int Paper() {
			return _paper;
		}

		public void AddMetal(int amount) {
			_metal += amount;
		}

		public void AddPaper(int amount) {
			_paper += amount;
		}
	}
}