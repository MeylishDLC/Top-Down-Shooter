using System;
using Bullets;
using Core;
using Core.InputSystem;
using Core.LevelSettings;
using Core.PoolingSystem;
using Core.SceneManagement;
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
    public class LevelSceneInstaller : MonoInstaller
    {
        [SerializeField] private InputListener inputListener;
        [SerializeField] private LevelChargesHandler levelChargesHandler;
        [SerializeField] private GameObject playerObject;
        [SerializeField] private Shop shop;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private WeaponsSetterConfig weaponsSetterConfig;
        [SerializeField] private CustomCursor customCursor;
        [SerializeField] private DialogueDisplay baseDialogueDisplay;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LevelDialogueConfig levelDialogueConfig;

        private DialogueManager _dialogueManager;
        public override void InstallBindings()
        {
            BindMainCamera();
            BindInputListener();
            BindSceneLoader();
            BindDialogueManager();
            BindProjectContext();
            BindPlayer();
            BindSpawner();
            BindLevelSetter();
            BindUIShopDisplay();
            BindPlayerWeaponsSetter();
            BindCustomCursor();
            BindLevelDialogues();
        }
        private void BindInputListener()
        {
            Container.Bind<InputListener>().FromInstance(inputListener).AsSingle();
        }
        private void BindSceneLoader()
        {
            Container.Bind<SceneLoader>().FromInstance(sceneLoader).AsSingle();
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
            Container.Bind<PlayerMovement>().FromComponentOn(playerObject).AsSingle();
        }
        private void BindSpawner()
        {
            Container.Bind<Spawner>().AsSingle();
        }
        private void BindLevelSetter()
        {
            Container.Bind<LevelChargesHandler>().FromInstance(levelChargesHandler).AsSingle();
        }
        private void BindUIShopDisplay()
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
    }
}