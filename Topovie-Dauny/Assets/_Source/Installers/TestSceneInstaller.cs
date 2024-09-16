using DialogueSystem;
using Player.PlayerMovement;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject player;
        [SerializeField] private PauseMenu pauseMenu;
        public override void InstallBindings()
        {
            BindDialogueManager();
            BindPlayer();
            BindPauseMenu();
        }

        private void BindDialogueManager()
        {
            Container.Bind<DialogueManager>().FromInstance(dialogueManager).AsSingle();
        }
        private void BindPlayer()
        {
            Container.Bind<PlayerMovement>().FromComponentOn(player).AsSingle();
        }
        private void BindPauseMenu()
        {
            Container.Bind<PauseMenu>().FromInstance(pauseMenu).AsSingle();
        }
    }
}