using System;
using System.Collections.Generic;
using Bullets;
using Bullets.Projectile;
using Core.PoolingSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Test_Gun;
using Zenject;

namespace Core.Bootstrappers
{
    public class TutorialSceneBootstrapper: MonoBehaviour
    {
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        [SerializeField] private BasicGun ppGun;
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair; 
        [SerializeField] private PoolConfigParentPair ppBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair enemyProjectileDataPair;
        
        private BulletPool _pistolBulletPool;
        private BulletPool _ppBulletPool;
        private ProjectilePool _enemyProjectilePool;

        private Spawner _spawner;
        private void Awake()
        {
            InitializePools();
            InitializeGuns();
        }
        private void OnDestroy()
        {
            CleanUpPools();
            _spawner.OnShootingEnemyInitialised -= InitializeEnemyProjectilePool;
        }

        [Inject]
        public void Construct(Spawner spawner)
        {
            _spawner = spawner;
            _spawner.OnShootingEnemyInitialised += InitializeEnemyProjectilePool;
        }
        private void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
            _enemyProjectilePool = new ProjectilePool(enemyProjectileDataPair.PoolConfig, enemyProjectileDataPair.PoolParent);
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
        }
        private void InitializeEnemyProjectilePool(Shooter shooter)
        {
            shooter.InitializePool(_enemyProjectilePool);
        }
        private void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
            _ppBulletPool.CleanUp();
        }
    }
}