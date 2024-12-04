using System.Threading;
using Bullets.BulletPatterns;
using Core.Data;
using Core.LevelSettings;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private NameObjectPair<BulletSpawner>[] bulletSpawners;
        
        private PoolInitializer _poolInitializer;
        
        [Inject]
        public void Construct(PoolInitializer poolInitializer)
        {
            _poolInitializer = poolInitializer;
        }
        protected override void Awake()
        {
            base.Awake();
            _poolInitializer.InitAll();
            Initialize(CancellationToken.None).Forget();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _poolInitializer.CleanUp();
        }
        private async UniTask Initialize(CancellationToken cancellationToken)
        {
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
            InitializeGuns();
            InitializeFireballSpawners();
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(1));
            ppGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(2));
            shotgun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(3));
            rpgGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(4));
        }
        private void InitializeFireballSpawners()
        {
            foreach (var spawner in bulletSpawners)
            {
                var pool = _poolInitializer.GetBulletPoolForEnemy(spawner.EnemyAssetName);
                spawner.Object.InjectPool(pool);
            }
        }
    }
}