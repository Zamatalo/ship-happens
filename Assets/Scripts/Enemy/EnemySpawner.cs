using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Enemy {
	public class EnemySpawner : MonoBehaviour {
		public List<GameObject> enemyPrefabs;
		public float spawnInterval = 60f; // Time in seconds between spawns
		public float minX; // Width of the area to spawn loot
		public float maxX; // Depth of the area to spawn loot
		public float minZ; // Width of the area to spawn loot
		public float maxZ; // Depth of the area to spawn loot

		void Start() {
			StartCoroutine(SpawnEnemyCoroutine());
		}

		IEnumerator SpawnEnemyCoroutine() {
			while (true) {
				if (NetworkManager.Singleton.IsListening) {
					SpawnEnemies();
					yield return new WaitForSeconds(spawnInterval);
				} else {
					yield return new WaitForSeconds(1);
				}
			}
		}

		void SpawnEnemies()
		{
			var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None); {
				var player = players[Random.Range(0, players.Length)];

					var spawnPosition = new Vector3(
						player.transform.position.x + Random.Range(minX, maxX),
						player.transform.position.y,
						player.transform.position.z + Random.Range(minZ, maxZ)
					);

					float randomX = Random.Range(0, 360);
					float randomY = Random.Range(0, 360);
					float randomZ = Random.Range(0, 360);

					var randomIndex = Random.Range(0, enemyPrefabs.Count);
					var selectedLootPrefab = enemyPrefabs[randomIndex];

					Instantiate(selectedLootPrefab, spawnPosition, Quaternion.Euler(randomX, randomY, randomZ));
			}
		}
	}
}