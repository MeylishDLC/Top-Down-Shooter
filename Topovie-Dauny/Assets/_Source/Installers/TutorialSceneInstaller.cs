using Bullets;
using Core;
using Core.PoolingSystem;
using DialogueSystem;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TutorialSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelChargesHandler levelChargesHandler;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject playerObject;
        [SerializeField] private UI.UIShop.Shop shop;
        
        public override void InstallBindings()
        {
            BindProjectContext();
            BindDialogueManager();
            BindPlayer();
            BindSpawner();
            BindLevelSetter();
            BindUIShopDisplay();
            BindPlayerWeaponsSetter();
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
            Container.Bind<WeaponsSetter>().AsSingle();
        }
    }
}