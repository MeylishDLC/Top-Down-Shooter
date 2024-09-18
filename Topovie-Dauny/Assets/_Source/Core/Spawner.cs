using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core
{
    //todo: meylish what the fuck refactor this later
    public static class Spawner
    {
        private static void SpawnRandomlyWithInjection(Transform[] spawnPoints, GameObject[] enemyPrefabs,
            SceneContext currentSceneContext)
        {
            var randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            currentSceneContext.Container.InstantiatePrefab(randomEnemy,
                GetRandomPositionWithinSpawnRange(randomSpawn, Random.Range(0.1f, 0.5f)),
                Quaternion.identity, randomSpawn);
        }
        public static async UniTask SpawnEnemiesRandomlyAsync(Transform[] spawnPoints, GameObject[] enemyPrefabs, SceneContext currentSceneContext, 
            int maxDelayBetweenSpawn, int minDelayBetweenSpawn, int maxEnemiesAtOnce, bool randomiseEnemiesAmount, CancellationToken token)
        {
            while (true)
            {
                var randomDelay = Random.Range(minDelayBetweenSpawn, maxDelayBetweenSpawn + 1);
                await UniTask.Delay(randomDelay, cancellationToken: token);
                
                if (randomiseEnemiesAmount)
                {
                    var randomAmount = Random.Range(1, maxEnemiesAtOnce+1);
                    for (var i = 0; i < randomAmount; i++)
                    {
                        SpawnRandomlyWithInjection(spawnPoints, enemyPrefabs, currentSceneContext);
                    }
                }
                else
                {
                    for (var i = 0; i < maxEnemiesAtOnce; i++)
                    {
                        SpawnRandomlyWithInjection(spawnPoints, enemyPrefabs, currentSceneContext);
                    }
                }
            }
        }
        private static Vector3 GetRandomPositionWithinSpawnRange(Transform spawn, float spawnRange)
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