using Notifications;
using UnityEngine;

// Make sure this namespace is accessible

namespace Player {
	public class PlayerNeeds : MonoBehaviour {
		public float maxHealth = 100f;
		[HideInInspector] public float health;
		public float maxHunger = 100f;
		[HideInInspector] public float hunger;
		public float maxThirst = 100f;
		[HideInInspector] public float thirst;
		public float maxOxygen = 100f;
		[HideInInspector] public float oxygen;

		private bool lowHungerNotified;
		private bool lowOxygenNotified;
		private bool lowThirstNotified;

		private NotificationManager notificationManager;

		private void Awake() {
			health = maxHealth;
			hunger = maxHunger;
			thirst = maxThirst;
			oxygen = maxOxygen;

			notificationManager = FindObjectOfType<NotificationManager>();
		}

		private void Update() {
			CheckNeeds();
		}

		private void FixedUpdate() {
			hunger = Mathf.Max(0, hunger - Time.fixedDeltaTime);
			thirst = Mathf.Max(0, thirst - Time.fixedDeltaTime);
			oxygen = Mathf.Max(0, oxygen - Time.fixedDeltaTime);
		}

		private void CheckNeeds() {
			if (hunger < maxHunger * 0.2f && !lowHungerNotified) {
				notificationManager?.ShowNotification("You’re starving! Find food ASAP!");
				lowHungerNotified = true;
			} else if (hunger >= maxHunger * 0.25f) {
				lowHungerNotified = false;
			}

			if (thirst < maxThirst * 0.2f && !lowThirstNotified) {
				notificationManager?.ShowNotification("Dehydration alert! Drink or die.");
				lowThirstNotified = true;
			} else if (thirst >= maxThirst * 0.25f) {
				lowThirstNotified = false;
			}

			if (oxygen < maxOxygen * 0.2f && !lowOxygenNotified) {
				notificationManager?.ShowNotification(
					"Running out of air. Hold your breath... if you can.");
				lowOxygenNotified = true;
			} else if (oxygen >= maxOxygen * 0.25f) {
				lowOxygenNotified = false;
			}
		}
	}
}