using System.Threading;
using Bullets;
using Bullets.BulletPatterns;
using Bullets.BulletPools;
using Bullets.Projectile;
using Core.LevelSettings;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weapons.Test_Gun;
using Zenject;

namespace Core.Bootstrappers
{
    public class TutorialLevelBootstrapper: MonoBehaviour, ILevelBootstrapper
    {
        [Header("Scene Load Stuff")] 
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        
        [Header("BULLET SPAWNS")]
        [SerializeField] private BulletSpawner bulletSpawner;
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair enemyBulletPoolDataPair;
        
        private BulletPool _pistolBulletPool;
        private EnemyBulletPool _enemyBulletPool;

        private SceneLoader _sceneLoader;
        private AssetLoader _environmentLoader = new();
        private async void Awake()
        {
            await InstantiateAssets(CancellationToken.None);
            InitializePools();
            InitializeGuns();
            bulletSpawner.Construct(_enemyBulletPool);
        }
        private void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
            CleanUpPools();
        }
        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        public async UniTask InstantiateAssets(CancellationToken token)
        {
            _sceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            _sceneLoader.SetLoadingScreenActive(false);
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
        public void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _enemyBulletPool = new EnemyBulletPool(enemyBulletPoolDataPair.PoolConfig, enemyBulletPoolDataPair.PoolParent);
        }
        public void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
        }
        public void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
        }
    }
}