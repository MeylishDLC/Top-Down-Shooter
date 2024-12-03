using System;
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
using DialogueSystem.LevelDialogue;
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
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair;
        
        private BulletPool _pistolBulletPool;
        private LevelDialogues _levelDialogues;
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, LevelDialogues levelDialogues)
        {
            _levelDialogues = levelDialogues;
        }
        protected override void Awake()
        {
            InstantiateAssets(CancellationToken.None).Forget();
           
            InitializePools();
            InitializeGuns();
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
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
        }
        private void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
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