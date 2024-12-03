using System;
using System.Collections.Generic;
using System.Threading;
using Bullets;
using Bullets.BulletPools;
using Bullets.Projectile;
using Core.LevelSettings;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using Enemies;
using Enemies.EnemyTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Weapons.Guns;
using Zenject;

namespace Core.Bootstrappers
{
    public class FirstLevelBootstrapper: BaseLevelBootstrapper
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
        private EnemyPoolInjector _enemyPoolInjector;

        private LevelDialogues _levelDialogues;
        private Spawner _spawner;
        
        [Inject]
        public void Construct(Spawner spawner, SceneLoader sceneLoader, LevelDialogues levelDialogues)
        {
            _levelDialogues = levelDialogues;
            _spawner = spawner;
        }
        protected override void Awake()
        {
            base.Awake();
            InitializePools();
            InitializeGuns();
        }
        private void Start()
        {
            _levelDialogues.PlayStartDialogue();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            CleanUpPools();
            _levelDialogues.CleanUp();
        }
        public void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
            _enemyProjectilePool = new ProjectilePool(enemyProjectileDataPair.PoolConfig, enemyProjectileDataPair.PoolParent);

            _enemyPoolInjector = new EnemyPoolInjector(_spawner, _enemyProjectilePool, null);
        }
        public void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
        }
        public void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
            _ppBulletPool.CleanUp();
            _enemyProjectilePool.CleanUp();
            _enemyPoolInjector.CleanUp();
        }
    }
}