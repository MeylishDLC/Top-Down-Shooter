using Core;
using DialogueSystem;
using Player.PlayerCombat;
using Player.PlayerControl;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelSetter levelSetter;
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
            Container.Bind<LevelSetter>().FromInstance(levelSetter).AsSingle();
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