using System.Threading;
using Bullets.BulletPools;
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
using Weapons.Guns;
using Zenject;

namespace Core.Bootstrappers
{
    public class ThirdLevelBootstrapper: BaseLevelBootstrapper
    {
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        [SerializeField] private BasicGun ppGun;
        [SerializeField] private Shotgun shotgun;
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair; 
        [SerializeField] private PoolConfigParentPair ppBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair shotgunBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair enemyProjectileDataPair;
        
        private BulletPool _pistolBulletPool;
        private BulletPool _ppBulletPool;
        private BulletPool _shotgunBulletPool;
        private ProjectilePool _enemyProjectilePool;

        private LevelDialogues _levelDialogues;
        private Spawner _spawner;
        private EnemyPoolInjector _enemyPoolInjector;
        
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
        private void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
            _shotgunBulletPool = new BulletPool(shotgunBulletPoolDataPair.PoolConfig, shotgunBulletPoolDataPair.PoolParent);
            _enemyProjectilePool = new ProjectilePool(enemyProjectileDataPair.PoolConfig, enemyProjectileDataPair.PoolParent);

            _enemyPoolInjector = new EnemyPoolInjector(_spawner, _enemyProjectilePool, null);
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
            shotgun.Initialize(_shotgunBulletPool);
        }
        private void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
            _ppBulletPool.CleanUp();
            _shotgunBulletPool.CleanUp();
            _enemyPoolInjector.CleanUp();
        }
    }
}