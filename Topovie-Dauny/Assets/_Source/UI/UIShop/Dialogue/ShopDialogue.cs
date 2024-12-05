using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

namespace UI.UIShop.Dialogue
{
    public class ShopDialogue
    {
        private TMP_Text _dialogueText;
        private int _typeSpeed;
        private string _currentText = "";
        
        public ShopDialogue(TMP_Text dialogueText, int typeSpeedMilliseconds)
        {
            _dialogueText = dialogueText;
            _typeSpeed = typeSpeedMilliseconds;
        }

        public async UniTask TypeDialogueAsync(string text, CancellationToken token)
        {
            try
            {
                _currentText = "";
                for (int i = 0; i <= text.Length; i++)
                {
                    _currentText = text.Substring(0, i);
                    _dialogueText.text = _currentText;
                    await UniTask.Delay(_typeSpeed, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
    }
}