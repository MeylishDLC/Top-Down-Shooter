using System;
using System.Threading;
using Core.LoadingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Core.Bootstrappers
{
    public class BaseLevelBootstrapper: MonoBehaviour
    {
        protected SceneLoader SceneLoader {get; private set;}
        
        [Header("Scene Load Stuff")] 
        [SerializeField] private AstarPath pathfinder;
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        [SerializeField] private Transform playerRespawnPoint;
        [SerializeField] private GameObject playerObject;
        
        private const float LoadDelay = 0.7f;
        private readonly AssetLoader _environmentLoader = new();
        
        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            SceneLoader = sceneLoader;
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
        protected virtual async UniTask InstantiateAssets(CancellationToken token)
        {
            SceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            if (SceneLoader.LastSceneIndex == SceneLoader.CurrentSceneIndex)
            {
                PutPlayerInRespawn();
            }
            ScanPaths();
            await UniTask.Delay(TimeSpan.FromSeconds(LoadDelay), cancellationToken: token);
            SceneLoader.SetLoadingScreenActive(false);
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
        private void PutPlayerInRespawn()
        {
            playerObject.transform.position = playerRespawnPoint.position;
        }
    }
}