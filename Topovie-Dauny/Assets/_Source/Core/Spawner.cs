using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies;
using Enemies.Projectile;
using Player.PlayerControl;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core
{
    public class Spawner
    {
        private EnemyWave _currentEnemyWave;
        private PlayerMovement _playerMovement;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
        }
        public void InitializeEnemyWave(EnemyWave enemyWave)
        {
            _currentEnemyWave = enemyWave;
        }
        public async UniTask SpawnEnemiesRandomlyAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var randomDelay = Random.Range(_currentEnemyWave.MinTimeBetweenSpawnMilliseconds, 
                    _currentEnemyWave.MaxTimeBetweenSpawnMilliseconds + 1);
                try
                {
                    await UniTask.Delay(randomDelay, cancellationToken: token);
                
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
            var randomSpawn = _currentEnemyWave.SpawnPoints[Random.Range(0, _currentEnemyWave.SpawnPoints.Length)];
            var randomEnemy = _currentEnemyWave.EnemyPrefabs[Random.Range(0, _currentEnemyWave.EnemyPrefabs.Length)];

            var enemy = Object.Instantiate(randomEnemy, randomSpawn.position, Quaternion.identity);
            enemy.transform.SetParent(randomSpawn);
            InitializeEnemy(enemy);
        }
        private void InitializeEnemy(GameObject enemy)
        {
            var enemyComponent = enemy.GetComponent<EnemyHealth>();
            enemyComponent.Construct(_playerMovement);

            if (enemy.TryGetComponent<Shooter>(out var shooter))
            {
                shooter.Construct(_playerMovement);
            }
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