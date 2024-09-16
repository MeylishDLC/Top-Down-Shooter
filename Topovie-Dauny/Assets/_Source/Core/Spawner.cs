﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core
{
    //todo: meylish what the fuck refactor this later
    public static class Spawner
    {
        public static void SpawnRandomlyWithInjection(Transform[] spawnPoints, GameObject[] enemyPrefabs, SceneContext currentSceneContext)
        {
            var randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
            var randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length - 1)];
                
            currentSceneContext.Container.InstantiatePrefab(randomEnemy, GetRandomPositionWithinSpawnRange(randomSpawn, Random.Range(0.1f, 0.5f)),
                Quaternion.identity, randomSpawn);
        }
        
        public static void SpawnEnemiesDuringTime(Transform[] spawnPoints, GameObject[] enemyPrefabs, SceneContext currentSceneContext,
            float spawnDuration, int delayBetweenSpawn, int maxEnemiesAtOnce, bool randomiseEnemiesAmount)
        {
            SpawnDuringTime(spawnPoints, enemyPrefabs, currentSceneContext, spawnDuration, delayBetweenSpawn, 
                maxEnemiesAtOnce, randomiseEnemiesAmount, CancellationToken.None).Forget();
        }

        private static async UniTask SpawnDuringTime(Transform[] spawnPoints, GameObject[] enemyPrefabs, SceneContext currentSceneContext,
            float spawnDuration, int delayBetweenSpawn, int maxEnemiesAtOnce, bool randomiseEnemiesAmount, CancellationToken token)
        {
            var startTime = Time.time;
            
            while (Time.time - startTime < spawnDuration)
            {
                await UniTask.Delay(delayBetweenSpawn, cancellationToken: token);

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