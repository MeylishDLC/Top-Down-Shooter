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
    public class BossLevelBootstrapper: BaseLevelBootstrapper
    {
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
        protected override void Awake()
        {
            base.Awake();
            InitializePools();
            InitializeGuns();
            InitializeFireballSpawners();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            CleanUpPools();
        }
        private void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(ppBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
            _shotgunBulletPool = new BulletPool(shotgunBulletPoolDataPair.PoolConfig, shotgunBulletPoolDataPair.PoolParent);
            _rpgBulletPool = new BulletPool(rpgBulletPoolDataPair.PoolConfig, rpgBulletPoolDataPair.PoolParent);
            _enemyBulletPool = new EnemyBulletPool(enemyBulletPoolDataPair.PoolConfig, enemyBulletPoolDataPair.PoolParent);
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
            shotgun.Initialize(_shotgunBulletPool);
            rpgGun.Initialize(_rpgBulletPool);
        }
        private void CleanUpPools()
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
    }
}