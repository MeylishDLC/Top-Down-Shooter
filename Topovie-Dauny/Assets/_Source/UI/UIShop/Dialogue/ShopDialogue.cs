using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoundSystem;
using SoundSystem.DialogueSoundSO;
using TMPro;
using Random = UnityEngine.Random;

namespace UI.UIShop.Dialogue
{
    public class ShopDialogue
    {
        private readonly TMP_Text _dialogueText;
        private readonly int _typeSpeed;
        private readonly DialogueAudioInfoSO _dialogueAudio;
        private readonly AudioManager _audioManager;
        
        public ShopDialogue(TMP_Text dialogueText, int typeSpeedMilliseconds, DialogueAudioInfoSO dialogueAudio,
            AudioManager audioManager)
        {
            _dialogueAudio = dialogueAudio;
            _dialogueText = dialogueText;
            _audioManager = audioManager;
            _typeSpeed = typeSpeedMilliseconds;
        }

        public async UniTask TypeDialogueAsync(string text, CancellationToken token)
        {
            try
            {
                _dialogueText.text = text;
                _dialogueText.maxVisibleCharacters = 0;

                foreach (var letter in text.ToCharArray())
                {
                    _dialogueText.maxVisibleCharacters++;
                    PlayDialogueSound(_dialogueText.maxVisibleCharacters);
                    await UniTask.Delay(_typeSpeed, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
        private void PlayDialogueSound(int currenDisplayedCharCount)
        {
            var typeSounds = _dialogueAudio.TypeSounds;
            var frequencyLvl = _dialogueAudio.FrequencyLvl;
            if (currenDisplayedCharCount % frequencyLvl == 0)
            {
                var randomIndex = Random.Range(0, typeSounds.Length);
                var soundClip = typeSounds[randomIndex];
                _audioManager.PlayOneShot(soundClip);
            }
        }
    }
}