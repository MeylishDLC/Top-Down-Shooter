using System;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;

namespace DialogueSystem.LevelDialogue
{
    public class LevelDialogues
    {
        private readonly LevelDialogueConfig _config;
        private readonly LevelChargesHandler _chargesHandler;
        private readonly DialogueManager _dialogueManager;
        private int _currentDialogueIndex;

        public LevelDialogues(LevelChargesHandler chargesHandler, DialogueManager dialogueManager, LevelDialogueConfig config)
        {
            _config = config;
            _chargesHandler = chargesHandler;
            _dialogueManager = dialogueManager;
            _chargesHandler.OnChargePassed += PlayDialogue;
        }
        public void PlayStartDialogue()
        {
            if (HasStartDialogue())
            {
                _dialogueManager.EnterDialogueMode(_config.DialogueOnStart);
            }
        }
        public bool HasStartDialogue()
        {
            return _config.DialogueOnStart;
        }
        public void CleanUp()
        {
            _chargesHandler.OnChargePassed -= PlayDialogue;
        }
        private void PlayDialogue()
        {
            if (_config.DialoguesAfterCharges.Length == 0
                || _currentDialogueIndex >= _config.DialoguesAfterCharges.Length)
            {
                return;
            }
            PlayDialogueAsync(CancellationToken.None).Forget();
        }
        private async UniTask PlayDialogueAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_config.DelayBeforeStartDialogue), cancellationToken: token);
            _dialogueManager.EnterDialogueMode(_config.DialoguesAfterCharges[_currentDialogueIndex]);
            _currentDialogueIndex++;
        }
    }
}