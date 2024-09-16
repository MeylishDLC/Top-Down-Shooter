using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core
{
     public class Spawner: MonoBehaviour
            {
                [SerializeField] private SceneContext sceneContext;
            
                [Header("Spawning")]
                [SerializeField] private Transform[] spawnPoints;
                [SerializeField] private float spawnRange;
                [SerializeField] private GameObject[] enemiesPrefabs; 
                [SerializeField] private GameObject[] bulletPatternsPrefabs;
            
                [Header("Timings")] 
                [SerializeField] private int timeBetweenSpawnMilliseconds;
                [SerializeField] private int spawningTimeMilliseconds;
            
    
                private void Start()
                {
                    StartSpawningAsync(CancellationToken.None).Forget();
                }
    
                private void SpawnRandomly()
                {
                    var randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                    var randomEnemy = enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length - 1)];
                
                    sceneContext.Container.InstantiatePrefab(randomEnemy, GetRandomPositionWithinSpawnRange(randomSpawn),
                        Quaternion.identity, randomSpawn);
                }
    
                private async UniTask StartSpawningAsync(CancellationToken token)
                {
                    while (true)
                    {
                        SpawnRandomly();
                        await UniTask.Delay(timeBetweenSpawnMilliseconds, cancellationToken: token);
                    }
                }
    
                private Vector3 GetRandomPositionWithinSpawnRange(Transform spawn)
                {
                    var randomOffset = new Vector3(
                        Random.Range(-spawnRange, spawnRange),
                        Random.Range(-spawnRange, spawnRange),
                        Random.Range(-spawnRange, spawnRange)
                    );
                    var spawnPosition = spawn.position + randomOffset;
                    return spawnPosition;
                }
            }
}