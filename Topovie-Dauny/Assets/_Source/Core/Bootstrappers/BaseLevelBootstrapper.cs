using System;
using System.Collections.Generic;
using System.Threading;
using Bullets.BulletPatterns;
using Core.Data;
using Core.InputSystem;
using Core.LevelSettings;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem.LevelDialogue;
using Enemies;
using Enemies.EnemyTypes;
using FMOD.Studio;
using SoundSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Weapons.Guns;
using Zenject;

namespace Core.Bootstrappers
{
    public class BaseLevelBootstrapper: MonoBehaviour
    {
        [Header("Guns")] 
        [SerializeField] private List<Gun> guns;
        
        [Header("Enemies")]
        [SerializeField] private List<EnemyContainer> containers;
        [SerializeField] private ShootingTower[] towers;
        
        [Header("Scene Load Stuff")] 
        [SerializeField] private AstarPath pathfinder;
        [SerializeField] private AssetReferenceGameObject environmentPrefab;
        [SerializeField] private Transform playerRespawnPoint;
        [SerializeField] private GameObject playerObject;
        
        private const float LoadDelay = 0.5f;
        private readonly AssetLoader _environmentLoader = new();
        private PoolInitializer _poolInitializer;
        private LevelChargesHandler _levelChargesHandler;
        private LevelDialogues _levelDialogues;
        private SceneLoader _sceneLoader;
        private AudioManager _audioManager;
        private InputListener _inputListener;
        
        [Inject]
        public void Construct(SceneLoader sceneLoader, PoolInitializer poolInitializer, LevelChargesHandler levelChargesHandler,
            LevelDialogues levelDialogues, AudioManager audioManager, InputListener inputListener)
        {
            _sceneLoader = sceneLoader;
            _poolInitializer = poolInitializer;
            _levelChargesHandler = levelChargesHandler;
            _levelDialogues = levelDialogues;
            _audioManager = audioManager;
            _inputListener = inputListener;
        }
        private void Awake()
        {
            LoadScene(CancellationToken.None).Forget();
        }
        private void OnDestroy()
        {
            _environmentLoader.ReleaseStoredInstance();
            _levelDialogues.CleanUp();
            _poolInitializer.CleanUp();
        }
        private void ScanPaths()
        {
            if (pathfinder)
            {
                pathfinder.Scan();
            }
        }
        protected virtual UniTask LoadScene(CancellationToken token)
        {
            _inputListener.SetInput(false);
            _sceneLoader.SetLoadingScreenActive(true);
            return InitializePools(token)
                .ContinueWith(() => InstantiateAssets(token))
                .ContinueWith(ShowScene);
        }
        private void ShowScene()
        {
            var levelNumber = SceneManager.GetActiveScene().buildIndex - 2;
            var music = _audioManager.FMODEvents.LevelsMusic[levelNumber];
            _inputListener.SetInput(true);
            
            _audioManager.ChangeMusic(music, STOP_MODE.ALLOWFADEOUT);
            _sceneLoader.SetLoadingScreenActive(false);

            _levelDialogues?.PlayStartDialogue();
        }
        private async UniTask InitializePools(CancellationToken cancellationToken)
        {
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
            _poolInitializer.InitAll();
            InitializeGuns();
            InitializeEnemyContainers();
            InitializeBulletSpawners();
            _levelChargesHandler.InitAllContainers(containers);
        }
        private async UniTask InstantiateAssets(CancellationToken token)
        {
            await InstantiateEnvironment(token);
            if (_sceneLoader.LastSceneIndex == _sceneLoader.CurrentSceneIndex)
            {
                PutPlayerInRespawn();
            }
            ScanPaths();
            await UniTask.Delay(TimeSpan.FromSeconds(LoadDelay), cancellationToken: token);
        }
        private void InitializeGuns()
        {
            for (int i = 0; i < guns.Count; i++)
            {
                guns[i].Initialize(_poolInitializer.GetBulletPoolForPlayerWeapon(i+1));
            }
        }
        private void InitializeEnemyContainers()
        {
            foreach (var container in containers)
            {
                var pool = _poolInitializer.GetEnemyPool(container.EnemyPrefabAssetName);
                container.InjectPool(pool);
            }
        }
        private void InitializeBulletSpawners()
        {
            if (towers == null)
            {
                return;
            }
            foreach (var tower in towers)
            {
                var pool = _poolInitializer.GetBulletPoolForEnemy(tower.EnemyBulletAssetName);
                tower.InjectPool(pool);
            }
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