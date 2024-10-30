using System;
using System.Threading;
using Bullets.Projectile;
using Core.EnemyWaveData;
using Cysharp.Threading.Tasks;
using Enemies;
using Player.PlayerControl;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core
{
    public class Spawner
    {
        public event Action<Shooter> OnShootingEnemyInitialised;
        
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
            var randomEnemy = GatRandomEnemy(randomPair);
            var randomSpawn = GetRandomSpawnPoint(randomPair);
            
            var enemy = Object.Instantiate(randomEnemy, randomSpawn.position, Quaternion.identity);
            enemy.transform.SetParent(randomSpawn);
            InitializeEnemy(enemy);
        }
        private EnemySpawnsPair GetRandomEnemySpawnsPair()
        {
            //todo add chance?
            var randomPair = _currentEnemyWave.EnemySpawnsPairs[Random.Range(0, _currentEnemyWave.EnemySpawnsPairs.Length)];
            return randomPair;
        }
        private Transform GetRandomSpawnPoint(EnemySpawnsPair enemySpawnsPair)
        {
            var randomSpawn = enemySpawnsPair.SpawnPoints[Random.Range(0, enemySpawnsPair.SpawnPoints.Length)];
            return randomSpawn;
        }
        private GameObject GatRandomEnemy(EnemySpawnsPair enemySpawnsPair)
        {
            var randomEnemy = enemySpawnsPair.EnemiesPrefabs[Random.Range(0, enemySpawnsPair.EnemiesPrefabs.Length)];
            return randomEnemy;
        }
        private void InitializeEnemy(GameObject enemy)
        {
            var enemyComponent = enemy.GetComponent<EnemyHealth>();
            enemyComponent.Construct(_playerMovement);

            if (enemy.TryGetComponent<Shooter>(out var shooter))
            {
                shooter.Construct(_playerMovement);
                OnShootingEnemyInitialised?.Invoke(shooter);
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