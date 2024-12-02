using System.Threading;
using Bullets.BulletPatterns;
using Bullets.BulletPools;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weapons.Guns;
using Zenject;

namespace Core.Bootstrappers
{
    public class BossLevelBootstrapper: MonoBehaviour, ILevelBootstrapper
    {
        [Header("Scene Load Stuff")] 
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        [SerializeField] private BasicGun ppGun;
        [SerializeField] private Shotgun shotgun;
        [SerializeField] private BasicGun rpgGun;
        
        [Header("FIREBALL SPAWNERS")]
        [SerializeField] private BulletSpawner[] spawners;
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair ppBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair shotgunBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair rpgBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair enemyBulletPoolDataPair;

        
        private BulletPool _pistolBulletPool;
        private BulletPool _ppBulletPool;
        private BulletPool _shotgunBulletPool;
        private BulletPool _rpgBulletPool;
        private EnemyBulletPool _enemyBulletPool;

        private SceneLoader _sceneLoader;
        private AssetLoader _environmentLoader = new();
        
        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        private async void Awake()
        {
            await InstantiateAssets(CancellationToken.None);
            InitializePools();
            InitializeGuns();
            InitializeFireballSpawners();
        }
        private void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
            CleanUpPools();
        }
        public async UniTask InstantiateAssets(CancellationToken token)
        {
            _sceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            _sceneLoader.SetLoadingScreenActive(false);
        }
       
        public void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(ppBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
            _shotgunBulletPool = new BulletPool(shotgunBulletPoolDataPair.PoolConfig, shotgunBulletPoolDataPair.PoolParent);
            _rpgBulletPool = new BulletPool(rpgBulletPoolDataPair.PoolConfig, rpgBulletPoolDataPair.PoolParent);
            _enemyBulletPool = new EnemyBulletPool(enemyBulletPoolDataPair.PoolConfig, enemyBulletPoolDataPair.PoolParent);
        }
        public void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
            shotgun.Initialize(_shotgunBulletPool);
            rpgGun.Initialize(_rpgBulletPool);
        }
        public void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
            _ppBulletPool.CleanUp();
            _shotgunBulletPool.CleanUp();
            _rpgBulletPool.CleanUp();
            _enemyBulletPool.CleanUp();
        }
        private void InitializeFireballSpawners()
        {
            foreach (var spawner in spawners)
            {
                spawner.InjectPool(_enemyBulletPool);
            }
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
    }
}