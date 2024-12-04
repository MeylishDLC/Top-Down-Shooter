﻿using System.Threading;
using Core.LevelSettings;
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
        private LevelChargesHandler _levelChargesHandler;
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, LevelDialogues levelDialogues, PoolInitializer poolInitializer,
            LevelChargesHandler levelChargesHandler)
        {
            _levelChargesHandler = levelChargesHandler;
            _poolInitializer = poolInitializer;
            _levelDialogues = levelDialogues;
        }
        protected override void Awake()
        {
            InstantiateAssets(CancellationToken.None).Forget();
            _poolInitializer.InitAll();
            Initialize(CancellationToken.None).Forget();
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
            InitializeGuns();
            InitializeEnemyContainers();
            _levelChargesHandler.InitAllContainers(containers);
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