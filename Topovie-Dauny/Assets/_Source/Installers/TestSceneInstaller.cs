using DialogueSystem;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private DialogueManager dialogueManager;
        public override void InstallBindings()
        {
            BindDialogueManager();
        }

        private void BindDialogueManager()
        {
            Container.Bind<DialogueManager>().FromInstance(dialogueManager).AsSingle();
        }
    }
}