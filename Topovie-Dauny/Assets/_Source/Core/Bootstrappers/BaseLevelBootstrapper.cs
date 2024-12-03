using System.Threading;
using Bullets.BulletPools;
using Core.LevelSettings;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Core.Bootstrappers
{
    public class BaseLevelBootstrapper: MonoBehaviour
    {
        [Header("Scene Load Stuff")] 
        [SerializeField] private AstarPath pathfinder;
        [SerializeField] private AssetReferenceGameObject environmentPrefab;

        private SceneLoader _sceneLoader;
        private readonly AssetLoader _environmentLoader = new();
        
        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        protected virtual async void Awake()
        {
            await InstantiateAssets(CancellationToken.None);
        }
        protected virtual void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
        }
        private void ScanPaths()
        {
            if (pathfinder != null)
            {
                pathfinder.Scan();
            }
        }
        private async UniTask InstantiateAssets(CancellationToken token)
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
    }
}