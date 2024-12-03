using System.Collections.Generic;
using System.Threading;
using Bullets;
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
using Enemies.EnemyTypes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
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
            InitializeGuns();
            InitializeEnemyContainers();
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