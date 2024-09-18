using Core;
using DialogueSystem;
using Player.PlayerMovement;
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
            BindDialogueManager();
            BindPlayer();
            BindLevelSetter();
            BindUIShopDisplay();
        }
        private void BindDialogueManager()
        {
            Container.Bind<DialogueManager>().FromInstance(dialogueManager).AsSingle();
        }
        private void BindPlayer()
        {
            Container.Bind<PlayerMovement>().FromComponentOn(playerObject).AsSingle();
        }
        private void BindLevelSetter()
        {
            Container.Bind<LevelSetter>().FromInstance(levelSetter).AsSingle();
        }
        private void BindUIShopDisplay()
        {
            Container.Bind<UI.UIShop.Shop>().FromInstance(shop).AsSingle();
        }
    }
}