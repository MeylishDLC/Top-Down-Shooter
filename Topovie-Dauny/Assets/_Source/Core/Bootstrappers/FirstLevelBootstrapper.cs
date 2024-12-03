using System.Threading;
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
    public class FirstLevelBootstrapper: BaseLevelBootstrapper
    {
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        [SerializeField] private BasicGun ppGun;
        
        [Header("ENEMIES")] 
        [SerializeField] private EnemyContainer[] containers;
        
        private LevelDialogues _levelDialogues;
        private PoolInitializer _poolInitializer;
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, LevelDialogues levelDialogues, PoolInitializer poolInitializer)
        {
            _poolInitializer = poolInitializer;
            _levelDialogues = levelDialogues;
        }
        protected override void Awake()
        {
            base.Awake();
            _poolInitializer.InitAll();
            InitializeAddressables(CancellationToken.None).Forget();
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
        private async UniTask InitializeAddressables(CancellationToken cancellationToken)
        {
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
            InitializeGuns();
            InitializeEnemyContainers();
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(1));
            ppGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(2));
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