using Core;
using DialogueSystem;
using Player.PlayerMovement;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelSetter levelSetter;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject player;
        [SerializeField] private UIShopDisplay uiShopDisplay;
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
            Container.Bind<PlayerMovement>().FromComponentOn(player).AsSingle();
        }
        private void BindLevelSetter()
        {
            Container.Bind<LevelSetter>().FromInstance(levelSetter).AsSingle();
        }
        private void BindUIShopDisplay()
        {
            Container.Bind<UIShopDisplay>().FromInstance(uiShopDisplay).AsSingle();
        }
    }
}