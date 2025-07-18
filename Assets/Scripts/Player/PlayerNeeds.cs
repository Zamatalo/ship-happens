using UnityEngine;

namespace Player {
	public class PlayerNeeds : MonoBehaviour {
		public float maxHealth = 100f;
		[HideInInspector] public float health;
		public float maxHunger = 100f;
		[HideInInspector] public float hunger;
		public float hungerDecreaseSpeed;
		public float maxThirst = 100f;
		[HideInInspector] public float thirst;
		public float thirstDecreaseSpeed;
		public float maxOxygen = 100f;
		[HideInInspector] public float oxygen;
		public float oxygenDecreaseSpeed;

		private void Awake() {
			health = maxHealth;
			hunger = maxHunger;
			thirst = maxThirst;
			oxygen = maxOxygen;
		}

		private void FixedUpdate() {
			hunger = Mathf.Max(0, hunger - hungerDecreaseSpeed * Time.fixedDeltaTime);
			thirst = Mathf.Max(0, thirst - thirstDecreaseSpeed * Time.fixedDeltaTime);
			oxygen = Mathf.Max(0, oxygen - oxygenDecreaseSpeed * Time.fixedDeltaTime);
		}
	}
}