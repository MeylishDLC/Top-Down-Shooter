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
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Weapons.Test_Gun;
using Zenject;

namespace Core.Bootstrappers
{
    public class FirstLevelBootstrapper: MonoBehaviour, ILevelBootstrapper
    {
        [Header("Scene Load Stuff")] 
        [SerializeField] private AstarPath pathfinder;
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        
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

        private SceneLoader _sceneLoader;
        private LevelDialogues _levelDialogues;
        private Spawner _spawner;
        private AssetLoader _environmentLoader = new();
        [Inject]
        public void Construct(Spawner spawner, SceneLoader sceneLoader, LevelDialogues levelDialogues)
        {
            _levelDialogues = levelDialogues;
            _sceneLoader = sceneLoader;
            _spawner = spawner;
            _spawner.OnShootingEnemyInitialised += InitializeEnemyProjectilePool;
        }
        private async void Awake()
        {
            await InstantiateAssets(CancellationToken.None);
            InitializePools();
            InitializeGuns();
        }
        private void Start()
        {
            _levelDialogues.PlayStartDialogue();
        }
        private void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
            CleanUpPools();
            _spawner.OnShootingEnemyInitialised -= InitializeEnemyProjectilePool;
            _levelDialogues.Expose();
        }
        public void ScanPaths()
        {
            pathfinder.Scan();
        }
        public async UniTask InstantiateAssets(CancellationToken token)
        {
            _sceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            ScanPaths();
            _sceneLoader.SetLoadingScreenActive(false);
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
        public void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
            _enemyProjectilePool = new ProjectilePool(enemyProjectileDataPair.PoolConfig, enemyProjectileDataPair.PoolParent);
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
        }
        private void InitializeEnemyProjectilePool(Shooter shooter)
        {
            shooter.InitializePool(_enemyProjectilePool);
        }
    }
}