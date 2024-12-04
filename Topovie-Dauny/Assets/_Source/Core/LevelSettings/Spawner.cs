using System;
using System.Threading;
using Core.Data;
using Cysharp.Threading.Tasks;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.LevelSettings
{
    public class Spawner
    {
        private EnemyWave _currentEnemyWave;
        public void InitializeEnemyWave(EnemyWave enemyWave)
        {
            _currentEnemyWave = enemyWave;
        }
        public async UniTask SpawnEnemiesRandomlyAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var randomDelay = Random.Range(_currentEnemyWave.MinTimeBetweenSpawn, 
                    _currentEnemyWave.MaxTimeBetweenSpawn + 1);
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(randomDelay), cancellationToken: token);
                
                    var randomAmount = Random.Range(_currentEnemyWave.MinEnemySpawnAtOnce, _currentEnemyWave.MaxEnemySpawnAtOnce+1);
                    for (var i = 0; i < randomAmount; i++)
                    { 
                        SpawnRandomly();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
        private void SpawnRandomly()
        {
            var randomPair = GetRandomEnemySpawnsPair();
            var randomEnemy = GetRandomEnemy(randomPair);
            var randomSpawn = GetRandomSpawnPoint(randomPair);

            if (randomEnemy)
            {
                randomEnemy.transform.position = randomSpawn.position;
            }
        }
        private EnemySpawnsPair GetRandomEnemySpawnsPair()
        {
            //todo add chance?
            var randomPair = _currentEnemyWave.Pairs[Random.Range(0, _currentEnemyWave.Pairs.Length)];
            return randomPair;
        }
        private Transform GetRandomSpawnPoint(EnemySpawnsPair enemySpawnsPair)
        {
            var randomSpawn = enemySpawnsPair.SpawnPoints[Random.Range(0, enemySpawnsPair.SpawnPoints.Length)];
            return randomSpawn;
        }
        private EnemyHealth GetRandomEnemy(EnemySpawnsPair enemySpawnsPair)
        {
            var container = enemySpawnsPair.Enemies[Random.Range(0, enemySpawnsPair.Enemies.Length)];
            container.Pool.TryGetFromPool(out var enemy);
            return enemy;
        }
        private Vector3 GetRandomPositionWithinSpawnRange(Transform spawn, float spawnRange)
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