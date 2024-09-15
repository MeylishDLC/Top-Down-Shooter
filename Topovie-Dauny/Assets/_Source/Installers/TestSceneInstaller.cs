using DialogueSystem;
using Player.PlayerMovement;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject player;
        public override void InstallBindings()
        {
            BindDialogueManager();
            BindPlayer();
        }

        private void BindDialogueManager()
        {
            Container.Bind<DialogueManager>().FromInstance(dialogueManager).AsSingle();
        }

        private void BindPlayer()
        {
            Container.Bind<PlayerMovement>().FromComponentOn(player).AsSingle();
        }
    }
}