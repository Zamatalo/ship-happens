using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class NetworkUi : MonoBehaviour {
		public GameObject hostButtonPrefab;
		public GameObject clientButtonPrefab;
		private Button _hostButton;
		private Button _clientButton;

		private void Start() {
			_hostButton = hostButtonPrefab.GetComponent<Button>();
			_clientButton = clientButtonPrefab.GetComponent<Button>();

			_hostButton.onClick.AddListener(StartHost);
			_clientButton.onClick.AddListener(StartClient);
		}

		private void StartHost() {
			NetworkManager.Singleton.StartHost();
			Destroy(_hostButton.gameObject);
			Destroy(_clientButton.gameObject);
		}

		private void StartClient() {
			NetworkManager.Singleton.StartClient();
			Destroy(_hostButton.gameObject);
			Destroy(_clientButton.gameObject);
		}
	}
}