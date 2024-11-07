using Bullets;
using Core;
using Core.InputSystem;
using Core.LevelSettings;
using Core.PoolingSystem;
using Core.SceneManagement;
using DialogueSystem;
using Player.PlayerCombat;
using Player.PlayerControl;
using UI.Core;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class LevelSceneInstaller : MonoInstaller
    {
        [SerializeField] private InputListener inputListener;
        [SerializeField] private LevelChargesHandler levelChargesHandler;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject playerObject;
        [SerializeField] private UI.UIShop.Shop shop;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private WeaponsSetterConfig weaponsSetterConfig;
        [SerializeField] private CustomCursor customCursor;
        
        public override void InstallBindings()
        {
            BindInputListener();
            BindSceneLoader();
            BindProjectContext();
            BindDialogueManager();
            BindPlayer();
            BindSpawner();
            BindLevelSetter();
            BindUIShopDisplay();
            BindPlayerWeaponsSetter();
            BindCustomCursor();
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
            Container.Bind<DialogueManager>().FromInstance(dialogueManager).AsSingle();
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
            Container.Bind<UI.UIShop.Shop>().FromInstance(shop).AsSingle();
        } 
        private void BindPlayerWeaponsSetter()
        {
            Container.Bind<WeaponsSetter>().AsSingle().WithArguments(weaponsSetterConfig);
        }
        private void BindCustomCursor()
        {
            Container.Bind<CustomCursor>().FromInstance(customCursor).AsSingle();
        }
    }
}