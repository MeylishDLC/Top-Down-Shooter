using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class Spawner: MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints; 
        [SerializeField] private GameObject[] enemiesPrefabs; 
        [SerializeField] private GameObject[] bulletPatternsPrefabs;

        [Header("Timings")] 
        [SerializeField] private int timeBetweenSpawnMilliseconds;
        [SerializeField] private int spawningTimeMilliseconds;

        private void SpawnRandomly()
        {
            var randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
            var randomEnemy = enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length - 1)];
            Instantiate(randomEnemy, randomSpawn);
        }
    }
}