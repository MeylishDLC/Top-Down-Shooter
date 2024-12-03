using Core.InputSystem;
using Core.LevelSettings;
using Core.PoolingSystem;
using DialogueSystem;
using DialogueSystem.LevelDialogue;
using Player.PlayerCombat;
using Player.PlayerControl;
using UI.Core;
using UI.UIShop;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ChargeLevelInstaller : MonoInstaller
    {
        [SerializeField] private InputListener inputListener;
        [SerializeField] private LevelChargesHandler levelChargesHandler;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private Shop shop;
        [SerializeField] private WeaponsSetterConfig weaponsSetterConfig;
        [SerializeField] private CustomCursor customCursor;
        [SerializeField] private DialogueDisplay baseDialogueDisplay;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LevelDialogueConfig levelDialogueConfig;
        [SerializeField] private PoolInitializerConfig poolInitializerConfig;

        private DialogueManager _dialogueManager;
        private PoolInitializer _poolInitializer;
        public override void InstallBindings()
        {
            BindPoolInitializer();
            BindMainCamera();
            BindInputListener();
            BindDialogueManager();
            BindProjectContext();
            BindPlayer();
            BindSpawner();
            BindStatesChanger();
            BindLevelSetter();
            BindShop();
            BindPlayerWeaponsSetter();
            BindCustomCursor();
            BindLevelDialogues();
        }
        private void OnDestroy()
        {
            _dialogueManager.CleanUp();
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
            _dialogueManager = new DialogueManager(inputListener, baseDialogueDisplay);
            Container.Bind<DialogueManager>().FromInstance(_dialogueManager).AsSingle();
        }
        private void BindPlayer()
        {
            Container.Bind<PlayerMovement>().FromInstance(playerMovement).AsSingle();
        }
        private void BindSpawner()
        {
            Container.Bind<Spawner>().AsSingle();
        }
        private void BindLevelSetter()
        {
            Container.Bind<LevelChargesHandler>().FromInstance(levelChargesHandler).AsSingle();
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
        private void BindLevelDialogues()
        {
            Container.Bind<LevelDialogues>().AsSingle()
                .WithArguments(levelChargesHandler, _dialogueManager, levelDialogueConfig);
        }

        private void BindPoolInitializer()
        {
            _poolInitializer = new PoolInitializer(poolInitializerConfig);
            Container.Bind<PoolInitializer>().FromInstance(_poolInitializer).AsSingle();
        }
    }
}