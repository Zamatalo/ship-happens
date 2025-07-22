using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Notifications {
    public class NotificationManager : MonoBehaviour {
        public GameObject notificationPanelPrefab;
        
        private void Start() {
            ShowNotification("Welcome to Ship happens!", 5f);
        }
        
        public void ShowNotification(string message, float duration = 5f) {
            var notif = Instantiate(notificationPanelPrefab);
            notif.GetComponentInChildren<TMP_Text>().text = message;

            StartCoroutine(FadeAndDestroy(notif, duration));
        }

        private static IEnumerator FadeAndDestroy(GameObject notif, float delay) {
            yield return new WaitForSeconds(delay);

            CanvasGroup canvasGroup = notif.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = notif.AddComponent<CanvasGroup>();
            }

            float fadeDuration = 0.5f;
            float elapsed = 0f;

            while (elapsed < fadeDuration) {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            Destroy(notif);
        }
    }
}