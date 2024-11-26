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
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weapons.Test_Gun;
using Zenject;

namespace Core.Bootstrappers
{
    public class TutorialLevelBootstrapper: MonoBehaviour, ILevelBootstrapper
    {
        [Header("Scene Load Stuff")] 
        [SerializeField] private AstarPath pathfinder;
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair;
        
        private BulletPool _pistolBulletPool;

        private LevelDialogues _levelDialogues;
        private SceneLoader _sceneLoader;
        private AssetLoader _environmentLoader = new();
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, LevelDialogues levelDialogues)
        {
            _sceneLoader = sceneLoader;
            _levelDialogues = levelDialogues;
        }
        private async void Awake()
        {
            await InstantiateAssets(CancellationToken.None);
            InitializePools();
            InitializeGuns();
            _levelDialogues.PlayStartDialogue();
        }
        private void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
            CleanUpPools();
            _levelDialogues.Expose();
        }
        public void ScanPaths()
        {
            pathfinder.Scan();
        }
        public async UniTask InstantiateAssets(CancellationToken token)
        {
            _sceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            ScanPaths();
            _sceneLoader.SetLoadingScreenActive(false);
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
        public void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
        }
        public void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
        }
        public void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
        }
    }
}