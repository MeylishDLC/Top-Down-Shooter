using System.Threading;
using Bullets.BulletPools;
using Core.Data;
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
        
        [Header("ENEMIES")] 
        [SerializeField] private EnemyContainer[] containers;
        
        private PoolInitializer _poolInitializer;
        private LevelDialogues _levelDialogues;
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, LevelDialogues levelDialogues, PoolInitializer poolInitializer)
        {
            _poolInitializer = poolInitializer;
            _levelDialogues = levelDialogues;
        }
        protected override void Awake()
        {
            base.Awake();
            InitializeGuns();
            InitializeEnemyContainers();
        }
        private void Start()
        {
            _levelDialogues.PlayStartDialogue();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _levelDialogues.CleanUp();
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(1));
            ppGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(2));
            shotgun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(3));
        }
        private void InitializeEnemyContainers()
        {
            foreach (var container in containers)
            {
                var pool = _poolInitializer.GetEnemyPool(container.EnemyPrefabAssetName);
                container.InjectPool(pool);
            }
        }
    }
}