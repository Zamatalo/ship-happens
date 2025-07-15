using Player;
using TMPro;
using UnityEngine;

namespace UI {
	public class ResourceAmountText : MonoBehaviour {
		public GameObject playerObj;
		private PlayerInventory _playerInventory;
		private TMP_Text _text;

		private void Start() {
			_playerInventory = playerObj.GetComponent<PlayerInventory>();
			_text = GetComponent<TMP_Text>();
		}

		private void Update() {
			_text.text = $"Metal: {_playerInventory.Metal()}";
		}
	}
}