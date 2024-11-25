using System.Threading;
using Bullets.BulletPools;
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
    public class BossLevelBootstrapper: MonoBehaviour, ILevelBootstrapper
    {
        [Header("Scene Load Stuff")] 
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        [SerializeField] private BasicGun ppGun;
        //todo add other guns
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair;
        [SerializeField] private PoolConfigParentPair ppBulletPoolDataPair;
        //todo add other pools
        
        private BulletPool _pistolBulletPool;
        private BulletPool _ppBulletPool;

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
        public async UniTask InstantiateAssets(CancellationToken token)
        {
            _sceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            _sceneLoader.SetLoadingScreenActive(false);
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
        public void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(ppBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
        }
        public void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
        }
        public void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
            _ppBulletPool.CleanUp();
        }
    }
}