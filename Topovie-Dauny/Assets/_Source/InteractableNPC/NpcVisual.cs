using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InteractableNPC
{
    public class NpcVisual
    {
        public event Action OnDisappearCompletely;
        
        private readonly float _npcFadeInDuration;
        private readonly float _npcFadeOutDuration;
        private readonly float _npcStayAfterDialogueDuration;
        private readonly NpcDialogueBubble _npcDialogueBubble;
        private readonly SpriteRenderer _npcRenderer;
        
        public NpcVisual(SpriteRenderer npcRenderer, NpcDialogueBubble npcDialogueBubble, 
            float fadeIn, float fadeOut, float stayDur)
        {
            _npcRenderer = npcRenderer;
            _npcDialogueBubble = npcDialogueBubble;
            _npcFadeInDuration = fadeIn;
            _npcFadeOutDuration = fadeOut;
            _npcStayAfterDialogueDuration = stayDur;

            _npcDialogueBubble.OnDialogueFinished += DisappearCompletely;
        }
        public void CleanUp()
        {
            _npcDialogueBubble.OnDialogueFinished -= DisappearCompletely;
        }
        public UniTask Disappear(CancellationToken token)
        {
            return _npcRenderer.DOFade(0f, _npcFadeOutDuration).ToUniTask(cancellationToken: token)
                .ContinueWith(() => _npcRenderer.gameObject.SetActive(false));
        }
        public UniTask Appear(CancellationToken token)
        {
            _npcRenderer.gameObject.SetActive(true);
            return _npcRenderer.DOFade(1f, _npcFadeInDuration).ToUniTask(cancellationToken: token);
        }
        private void DisappearCompletely()
        {
            DisappearCompletelyAsync(CancellationToken.None).Forget();
        }
        private async UniTask DisappearCompletelyAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_npcStayAfterDialogueDuration), cancellationToken: token);
            await _npcDialogueBubble.Disappear(token);
            await _npcRenderer.DOFade(0f, _npcFadeOutDuration).ToUniTask(cancellationToken: token);
            OnDisappearCompletely?.Invoke();
        }
    }
}