using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Loot {
	public class LootSpawner : MonoBehaviour {
		public GameObject lootPrefab; // Assign your loot prefab in the Inspector
		public float spawnInterval = 5f; // Time in seconds between spawns
		public float minX; // Width of the area to spawn loot
		public float maxX; // Depth of the area to spawn loot
		public float minY;
		public float maxY;
		public float minZ; // Width of the area to spawn loot
		public float maxZ; // Depth of the area to spawn loot
		public float minVelX;
		public float maxVelX;
		public float minVelZ;
		public float maxVelZ;
		public float rotSpeed;
		public int minLoot;
		public int maxLoot;

		void Start() {
			StartCoroutine(SpawnLootCoroutine());
		}

		IEnumerator SpawnLootCoroutine() {
			while (true) {
				// Check if the NetworkManager is listening
				if (NetworkManager.Singleton.IsListening) {
					SpawnLoot();
				}
				yield return
					new WaitForSeconds(
						spawnInterval); // Wait for the specified interval before spawning again
			}
		}

		void SpawnLoot() {
			int numberOfLootsToSpawn =
				Random.Range(minLoot, maxLoot); // Random number between 1 and 20

			for (int i = 0; i < numberOfLootsToSpawn; i++) {
				Vector3 spawnPosition = new Vector3(
					Random.Range(minX, maxX),
					Random.Range(minY, maxY),
					Random.Range(minZ, maxZ)
				);

				float randomX = Random.Range(0, 360);
				float randomY = Random.Range(0, 360);
				float randomZ = Random.Range(0, 360);

				GameObject lootInstance = Instantiate(lootPrefab, spawnPosition,
					Quaternion.Euler(randomX, randomY, randomZ));
				var loot = lootInstance.GetComponent<Loot>();
				loot.Init(minVelX, maxVelX, minVelZ, maxVelZ, rotSpeed);

				// Get the NetworkObject component
				NetworkObject networkObject = lootInstance.GetComponent<NetworkObject>();
				if (networkObject != null) {
					// Spawn the object over the network
					networkObject.Spawn();
				} else {
					Debug.LogError("NetworkObject component is missing on the loot prefab.");
				}
			}
		}
	}
}