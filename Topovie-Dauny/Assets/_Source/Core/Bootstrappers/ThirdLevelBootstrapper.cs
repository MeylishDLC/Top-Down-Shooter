using System.Threading;
using Core.LevelSettings;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using Enemies;
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
        private LevelChargesHandler _levelChargesHandler;
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, LevelDialogues levelDialogues, PoolInitializer poolInitializer,
            LevelChargesHandler levelChargesHandler)
        {
            _poolInitializer = poolInitializer;
            _levelDialogues = levelDialogues;
            _levelChargesHandler = levelChargesHandler;
        }
        protected override void Awake()
        {
            base.Awake();
            Initialize(CancellationToken.None).Forget();
        }
        private void Start()
        {
            _levelDialogues.PlayStartDialogue();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _levelDialogues.CleanUp();
            _poolInitializer.CleanUp();
        }
        private async UniTask Initialize(CancellationToken cancellationToken)
        {
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
            _poolInitializer.InitAll();
            InitializeGuns();
            InitializeEnemyContainers();
            _levelChargesHandler.InitAllContainers(containers);
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