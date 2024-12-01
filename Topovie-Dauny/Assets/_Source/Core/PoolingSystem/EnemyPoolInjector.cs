using System;
using Bullets.BulletPatterns;
using Bullets.BulletPools;
using Core.LevelSettings;
using Enemies;
using Enemies.EnemyTypes;
using UnityEngine;

namespace Core.PoolingSystem
{
    public class EnemyPoolInjector
    {
        private Spawner _spawner;
        
        private readonly ProjectilePool _enemyProjectilePool;
        private readonly EnemyBulletPool _enemyBulletPool;
        
        public EnemyPoolInjector(Spawner spawner, ProjectilePool enemyProjectilePool, EnemyBulletPool enemyBulletPool)
        {
            _enemyProjectilePool = enemyProjectilePool;
            _enemyBulletPool = enemyBulletPool;
            
            _spawner = spawner;
            _spawner.OnPoolUserSpawned += InjectPool;
        }
        public void CleanUp()
        {
            _spawner.OnPoolUserSpawned -= InjectPool;
        }
        private void InjectPool(IPoolUser poolUser)
        {
            if (poolUser is BulletSpawner bulletSpawner)
            {
                if (_enemyBulletPool == null)
                {
                    throw new PoolWasNullException($"Pool type: {typeof(EnemyBulletPool)}, pool user: {typeof(BulletSpawner)}");
                }
                bulletSpawner.InjectPool(_enemyBulletPool);
            }
            else if (poolUser is Shooter shooter)
            {
                if (_enemyProjectilePool == null)
                {
                    throw new PoolWasNullException($"Pool type: {typeof(ProjectilePool)}, pool user: {typeof(Shooter)}");
                }
                shooter.InjectPool(_enemyProjectilePool);
            }
            else if (poolUser is ShootingTower shootingTower)
            {
                if (_enemyBulletPool == null)
                {
                    throw new PoolWasNullException($"Pool type: {typeof(EnemyBulletPool)}, pool user: {typeof(ShootingTower)}");
                }
                shootingTower.InjectPool(_enemyBulletPool);
            }
            else
            {
                throw new Exception($"Pool User type of {poolUser.GetType()} is not supported to injection.");
            }
        }
        private sealed class PoolWasNullException: Exception
        {
            public override string Message { get; } = "The pool has not been initialized. Initialize it in bootstrap.";

            public PoolWasNullException(string additionToMessage)
            {
                Message += additionToMessage;
            }
        }

        
    }
}