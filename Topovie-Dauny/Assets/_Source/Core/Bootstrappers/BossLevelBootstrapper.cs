using System;
using System.Collections.Generic;
using System.Threading;
using Bullets.BulletPatterns;
using Core.Data;
using Core.LevelSettings;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using Enemies;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Weapons.Guns;
using Zenject;

namespace Core.Bootstrappers
{
    public class BossLevelBootstrapper: MonoBehaviour
    {
        [Header("Guns")] 
        [SerializeField] private List<Gun> guns;
        
        [FormerlySerializedAs("fireballs")]
        [Header("Fireball Spawners")]
        [SerializeField] private FireballSpawnerNamePair[] fireballsSpawners;
        
        [Header("Scene Load Stuff")] 
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        [SerializeField] private Transform playerRespawnPoint;
        [SerializeField] private GameObject playerObject;
        
        private const float LoadDelay = 0.7f;
        private readonly AssetLoader _environmentLoader = new();
        private PoolInitializer _poolInitializer;
        private SceneLoader _sceneLoader;
        
        [Inject]
        public void Construct(PoolInitializer poolInitializer, SceneLoader sceneLoader)
        {
            _poolInitializer = poolInitializer;
            _sceneLoader = sceneLoader;
        }
        private void Awake()
        {
            _sceneLoader.SetLoadingScreenActive(true);
            Initialize(CancellationToken.None)
                .ContinueWith(() => InstantiateAssets(CancellationToken.None))
                .ContinueWith(() => _sceneLoader.SetLoadingScreenActive(false)).Forget();
        }
        private void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
            _poolInitializer.CleanUp();
        }
        private async UniTask Initialize(CancellationToken cancellationToken)
        {
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
            _poolInitializer.InitAll();
            InitializeGuns();
            InitializeFireballSpawners();
        }
        private void InitializeFireballSpawners()
        {
            foreach (var spawner in fireballsSpawners)
            {
                var pool = _poolInitializer.GetBulletPoolForEnemy(spawner.FireballAssetName);
                spawner.BulletSpawner.InjectPool(pool);
            }
        }
        private void InitializeGuns()
        {
            for (int i = 0; i < guns.Count; i++)
            {
                guns[i].Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(i+1));
            }
        }
        private async UniTask InstantiateAssets(CancellationToken token)
        {
            _sceneLoader.SetLoadingScreenActive(true);
            await InstantiateEnvironment(token);
            if (_sceneLoader.LastSceneIndex == _sceneLoader.CurrentSceneIndex)
            {
                PutPlayerInRespawn();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(LoadDelay), cancellationToken: token);
            _sceneLoader.SetLoadingScreenActive(false);
        }
        private async UniTask InstantiateEnvironment(CancellationToken token)
        {
            await _environmentLoader.LoadGameObject(environmentPrefab, token).ContinueWith(Instantiate);
        }
        private void PutPlayerInRespawn()
        {
            playerObject.transform.position = playerRespawnPoint.position;
        }

        [System.Serializable]
        public class FireballSpawnerNamePair
        {
            [field:SerializeField] public string FireballAssetName {get; private set;}
            [field:SerializeField] public BulletSpawner BulletSpawner {get; private set;}
        }
    }
}