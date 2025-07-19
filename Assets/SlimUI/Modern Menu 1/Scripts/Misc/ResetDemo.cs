using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlimUI.Modern_Menu_1.Scripts.Misc {
	public class ResetDemo : MonoBehaviour {
		private void Update() {
			if (Input.GetKeyDown("r")) {
				SceneManager.LoadScene(0);
			}
		}
	}
}