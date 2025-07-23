using System.Collections;
using TMPro;
using UnityEngine;

namespace Achievements {
    public class AchievementManager : MonoBehaviour {
        public GameObject achievementPanelPrefab;
        private GameObject _achievementPanel;

        public void ShowAchievement(string message, float duration = 5f) {
            if (!ReferenceEquals(_achievementPanel, null)) Destroy(_achievementPanel);

            _achievementPanel = Instantiate(achievementPanelPrefab);
            _achievementPanel.GetComponentInChildren<TMP_Text>().text = message;

            StartCoroutine(FadeAndDestroy(duration));
        }

        private IEnumerator FadeAndDestroy(float delay) {
            yield return new WaitForSeconds(delay);

            var canvasGroup = _achievementPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = _achievementPanel.AddComponent<CanvasGroup>();

            var fadeDuration = 0.5f;
            var elapsed = 0f;

            while (elapsed < fadeDuration) {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            Destroy(_achievementPanel);
        }
    }
}