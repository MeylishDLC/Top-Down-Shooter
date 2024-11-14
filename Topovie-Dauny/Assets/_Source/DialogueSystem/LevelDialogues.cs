using System;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DialogueSystem
{
    public class LevelDialogues: MonoBehaviour
    {
        [SerializeField] private TextAsset dialogueOnStart;
        [SerializeField] private TextAsset[] dialogues;
        [SerializeField] private float delayBeforeStartDialogue;

        private int _currentDialogueIndex;
        private LevelChargesHandler _chargesHandler;
        private DialogueManager _dialogueManager;
        
        [Inject]
        public void Construct(LevelChargesHandler chargesHandler, DialogueManager dialogueManager)
        {
            _chargesHandler = chargesHandler;
            _dialogueManager = dialogueManager;
        }
        private void Awake()
        {
            if (dialogueOnStart != null)
            {
                _dialogueManager.EnterDialogueMode(dialogueOnStart);
            }
            _chargesHandler.OnChargePassed += PlayDialogue;
        }
        private void PlayDialogue()
        {
            PlayDialogueAsync(CancellationToken.None).Forget();
        }
        private async UniTask PlayDialogueAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeStartDialogue), cancellationToken: token);
            _dialogueManager.EnterDialogueMode(dialogues[_currentDialogueIndex]);
            _currentDialogueIndex++;
        }
    }
}