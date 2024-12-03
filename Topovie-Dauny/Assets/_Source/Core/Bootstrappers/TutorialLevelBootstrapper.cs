using System;
using System.Threading;
using Bullets;
using Bullets.BulletPatterns;
using Bullets.BulletPools;
using Bullets.Projectile;
using Core.Data;
using Core.LevelSettings;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using Enemies;
using UI.Tutorial;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weapons.Guns;
using Zenject;

namespace Core.Bootstrappers
{
    public class TutorialLevelBootstrapper: BaseLevelBootstrapper
    {
        [SerializeField] private BasicTutorial tutorial;
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        
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
            InstantiateAssets(CancellationToken.None).Forget();
            _poolInitializer.InitAll();
            InitializeGuns();
            InitializeEnemyContainers();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _levelDialogues.CleanUp();
            _poolInitializer.CleanUp();
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(1));
        }
        private void InitializeEnemyContainers()
        {
            foreach (var container in containers)
            {
                var pool = _poolInitializer.GetEnemyPool(container.EnemyPrefabAssetName);
                container.InjectPool(pool);
            }
        }
        protected override async UniTask InstantiateAssets(CancellationToken token)
        {
            await base.InstantiateAssets(token);
            if (SceneLoader.CurrentSceneIndex != SceneLoader.LastSceneIndex)
            {
                tutorial.EnableTutorial();
            }
            else
            {
                _levelDialogues.PlayStartDialogue();
            }
        }
    }
}