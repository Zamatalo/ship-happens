// using System.Collections;
// using System.Collections.Generic;
// using Player;
// using Unity.Netcode;
// using UnityEngine;
//
// namespace Loot {
// 	public class LootSpawner : MonoBehaviour {
// 		public List<GameObject> lootPrefabs; // Assign multiple loot prefabs in the Inspector
// 		// Assign your loot prefab in the Inspector
// 		public float spawnInterval = 5f; // Time in seconds between spawns
// 		public float minX; // Width of the area to spawn loot
// 		public float maxX; // Depth of the area to spawn loot
// 		public float minY;
// 		public float maxY;
// 		public float minZ; // Width of the area to spawn loot
// 		public float maxZ; // Depth of the area to spawn loot
// 		public float minVelX;
// 		public float maxVelX;
// 		public float minVelZ;
// 		public float maxVelZ;
// 		public float rotSpeed;
// 		public int minLoot;
// 		public int maxLoot;
//
// 		void Start() {
// 			StartCoroutine(SpawnLootCoroutine());
// 		}
//
// 		IEnumerator SpawnLootCoroutine() {
// 			while (true) {
// 				if (NetworkManager.Singleton.IsListening) {
// 					SpawnLoot();
// 					yield return new WaitForSeconds(spawnInterval);
// 				} else {
// 					yield return new WaitForSeconds(1);
// 				}
// 			}
// 		}
//
// 		void SpawnLoot() {
// 			foreach (var player in FindObjectsByType<PlayerController>(FindObjectsSortMode.None)) {
// 				var numberOfLootsToSpawn =
// 					Random.Range(minLoot, maxLoot);
//
// 				for (var i = 0; i < numberOfLootsToSpawn; i++) {
// 					var spawnPosition = new Vector3(
// 						player.transform.position.x + Random.Range(minX, maxX),
// 						player.transform.position.y + Random.Range(minY, maxY),
// 						player.transform.position.z + Random.Range(minZ, maxZ)
// 					);
//
// 					float randomX = Random.Range(0, 360);
// 					float randomY = Random.Range(0, 360);
// 					float randomZ = Random.Range(0, 360);
//
// 					var randomIndex = Random.Range(0, lootPrefabs.Count);
// 					var selectedLootPrefab = lootPrefabs[randomIndex];
//
// 					var inst = Instantiate(selectedLootPrefab, spawnPosition,
// 						Quaternion.Euler(randomX, randomY, randomZ));
//
// 					var loot = inst.GetComponent<Loot>();
// 					loot.Init(minVelX, maxVelX, minVelZ, maxVelZ, rotSpeed);
// 				}
// 			}
// 		}
// 	}
// }