using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Enemy {
	public class EnemySpawner : MonoBehaviour {
		public List<GameObject> enemyPrefabs;
		public float spawnInterval = 60f;
		public float minX;
		public float maxX;
		public float minZ;
		public float maxZ;
		private ProgressManager _pm;

		private void Start() {
			_pm = FindAnyObjectByType<ProgressManager>();
			StartCoroutine(SpawnEnemyCoroutine());
		}

		private IEnumerator SpawnEnemyCoroutine() {
			while (true) {
				if (NetworkManager.Singleton.IsListening) {
					if (_pm.state < ProgressManager.State.Venus) {
						yield return new WaitForSeconds(1);
					} else {
						SpawnEnemies();
						yield return new WaitForSeconds(spawnInterval);
					}
				} else {
					yield return new WaitForSeconds(1);
				}
			}
		}

		private void SpawnEnemies() {
			var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
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

			Instantiate(selectedLootPrefab, spawnPosition,
				Quaternion.Euler(randomX, randomY, randomZ));
		}
	}
}