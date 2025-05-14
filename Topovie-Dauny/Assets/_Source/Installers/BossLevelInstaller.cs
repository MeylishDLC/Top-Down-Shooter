using Core.InputSystem;
using Core.LevelSettings;
using Core.PoolingSystem;
using Core.PoolingSystem.Configs;
using DialogueSystem;
using GameEnvironment.ShopLogic.UIShop;
using Player.PlayerCombat;
using Player.PlayerControl;
using SoundSystem;
using UI.Core;
using UI.Menus;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Installers
{
    public class BossLevelInstaller: MonoInstaller
    {
        [SerializeField] private InputListener inputListener;
        [SerializeField] private Shop shop;
        [SerializeField] private WeaponsSetterConfig weaponsSetterConfig;
        [SerializeField] private CustomCursor customCursor;
        [SerializeField] private DialogueDisplay baseDialogueDisplay;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PoolInitializerConfig poolInitializerConfig;
        [SerializeField] private PauseMenu pauseMenu;
        
        [Header("Player Components")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Material playerDamagedMaterial;
        [SerializeField] private Volume playerVignetteVolume;
        
        private PoolInitializer _poolInitializer;
        private DialogueManager _dialogueManager;
        public override void InstallBindings()
        {
            BindMainCamera();
            BindInputListener();
            
            BindDialogueManager();
            BindProjectContext();
            BindPlayer();
            BindStatesChanger();
            
            BindCustomCursor();
            BindPauseMenu();
            BindShop();
            
            BindPlayerWeaponsSetter();
            BindPoolInitializer();
        }
        private void OnDestroy()
        {
            _dialogueManager.CleanUp();
        }
        private void BindPlayer()
        {
            Container.BindInstance(playerConfig).AsSingle();
            Container.Bind<PlayerMovement>().FromInstance(playerMovement).AsSingle();
            Container.Bind<Material>().FromInstance(playerDamagedMaterial).AsSingle();
            Container.Bind<Volume>().FromInstance(playerVignetteVolume).AsSingle();
            
            Container.Bind<PlayerDamagedDisplay>().AsSingle();
        }
        private void BindInputListener()
        {
            Container.Bind<InputListener>().FromInstance(inputListener).AsSingle();
        }
        private void BindProjectContext()
        {
            var context = FindFirstObjectByType<ProjectContext>();
            Container.Bind<ProjectContext>().FromInstance(context).AsSingle();
        }
        private void BindDialogueManager()
        {
            _dialogueManager = new DialogueManager(inputListener, baseDialogueDisplay, Container.Resolve<AudioManager>());
            Container.Bind<DialogueManager>().FromInstance(_dialogueManager).AsSingle();
        }
        private void BindStatesChanger()
        {
            Container.Bind<StatesChanger>().AsSingle();
        }
        private void BindShop()
        {
            Container.Bind<Shop>().FromInstance(shop).AsSingle();
        } 
        private void BindPlayerWeaponsSetter()
        {
            Container.Bind<WeaponsSetter>().AsSingle().WithArguments(weaponsSetterConfig);
        }
        private void BindCustomCursor()
        {
            Container.Bind<CustomCursor>().FromInstance(customCursor).AsSingle();
        }
        private void BindMainCamera()
        {
            Container.Bind<Camera>().FromInstance(mainCamera).AsSingle();
        }
        private void BindPoolInitializer()
        {
            _poolInitializer = new PoolInitializer(poolInitializerConfig);
            Container.Bind<PoolInitializer>().FromInstance(_poolInitializer).AsSingle();
        }
        private void BindPauseMenu()
        {
            Container.Bind<PauseMenu>().FromInstance(pauseMenu).AsSingle();
        }
    }
}