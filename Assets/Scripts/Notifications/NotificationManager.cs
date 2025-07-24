using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 * TODO: add array of notifications, because if notifications trigger often, they will interrupt each other
 */
namespace Notifications {
	public class NotificationManager : MonoBehaviour {
		public GameObject notificationPanelPrefab;
		private GameObject _notificationPanel;

		private void Start() {
			ShowNotification("Welcome to Ship happens!");
		}

		public void ShowNotification(string message, float duration = 5f) {
			if (!ReferenceEquals(_notificationPanel, null)) Destroy(_notificationPanel);

			_notificationPanel = Instantiate(notificationPanelPrefab);
			_notificationPanel.GetComponentInChildren<TMP_Text>().text = message;

			StartCoroutine(FadeAndDestroy(duration));
		}

		private IEnumerator FadeAndDestroy(float delay) {
			yield return new WaitForSeconds(delay);

			if (_notificationPanel.IsDestroyed()) yield break;

			var canvasGroup = _notificationPanel.AddComponent<CanvasGroup>();

			var fadeDuration = 0.5f;
			var elapsed = 0f;

			while (elapsed < fadeDuration) {
				if (_notificationPanel.IsDestroyed()) yield break;

				elapsed += Time.deltaTime;
				canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
				yield return null;
			}

			Destroy(_notificationPanel);
		}
	}
}