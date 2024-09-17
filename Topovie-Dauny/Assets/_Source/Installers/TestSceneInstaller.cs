using Core;
using DialogueSystem;
using Player.PlayerMovement;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelSetter levelSetter;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject player;
        public override void InstallBindings()
        {
            BindDialogueManager();
            BindPlayer();
            BindLevelSetter();
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
    }
}