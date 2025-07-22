using System.Collections;
using UnityEngine;

namespace Notifications {
	public class NotificationPanel : MonoBehaviour {
		public float fadeInDuration = 0.3f;
		private RectTransform _rect;
		private float _targetWidth;

		private void Start() {
			_rect = GetComponent<RectTransform>();
			_targetWidth = _rect.sizeDelta.x;

			StartCoroutine(Animate());
		}

		private IEnumerator Animate() {
			var startWidth = 0f;
			var elapsedTime = 0f;

			_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startWidth);

			while (elapsedTime < fadeInDuration) {
				var newWidth = Mathf.Lerp(startWidth, _targetWidth, elapsedTime / fadeInDuration);
				_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _targetWidth);
		}
	}
}