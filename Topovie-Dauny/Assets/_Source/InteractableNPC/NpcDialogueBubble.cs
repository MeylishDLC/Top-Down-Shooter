using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace InteractableNPC
{
    public class NpcDialogueBubble: MonoBehaviour
    {
        public event Action OnDialogueFinished;
        
        [TextArea] [SerializeField] private string dialogueText;
        [SerializeField] private float dialogueTypeSpeed;
        [SerializeField] private float bubbleFadeInDuration;
        [SerializeField] private float bubbleFadeOutDuration;

        private SpriteRenderer _dialogueBubbleRenderer;
        private TMP_Text _text;
        private void Awake()
        {
            _dialogueBubbleRenderer = GetComponent<SpriteRenderer>();
            _text = GetComponentInChildren<TMP_Text>();
            _dialogueBubbleRenderer.DOFade(0f, 0f);
            _text.gameObject.SetActive(false);
        }
        public async UniTask DisplayDialogueAsync(CancellationToken token)
        {
            await ShowDialogueBubbleAsync(token);
            await TypeDialogueAsync(token);
            OnDialogueFinished?.Invoke();
        }

        public UniTask Disappear(CancellationToken token)
        {
            var tasks = new List<UniTask>()
            {
                _dialogueBubbleRenderer.DOFade(0f, bubbleFadeOutDuration).ToUniTask(cancellationToken: token),
                _text.DOFade(0f, bubbleFadeOutDuration).ToUniTask(cancellationToken: token),
            };
            return UniTask.WhenAll(tasks);
        }
        private async UniTask ShowDialogueBubbleAsync(CancellationToken token)
        {
            await _dialogueBubbleRenderer.DOFade(1f, bubbleFadeInDuration).ToUniTask(cancellationToken: token);
        }

        private async UniTask TypeDialogueAsync(CancellationToken token)
        {
            _text.gameObject.SetActive(true);
            _text.text = dialogueText;
            _text.maxVisibleCharacters = 0;
            
            try
            {
                foreach (var letter in dialogueText.ToCharArray())
                {
                    _text.maxVisibleCharacters++;
                    await UniTask.Delay(TimeSpan.FromSeconds(dialogueTypeSpeed),
                        cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
    }
}