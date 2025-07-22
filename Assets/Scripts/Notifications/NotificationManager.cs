using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Notifications {
    public class NotificationManager : MonoBehaviour {
        public GameObject notificationPanelPrefab;

        public void ShowNotification(string message, float duration = 5f) {
            var notif = Instantiate(notificationPanelPrefab);
            notif.GetComponentInChildren<TMP_Text>().text = message;

            StartCoroutine(FadeAndDestroy(notif, duration));
        }

        private static IEnumerator FadeAndDestroy(GameObject notif, float delay) {
            yield return new WaitForSeconds(delay);
            Destroy(notif);
        }
    }
}