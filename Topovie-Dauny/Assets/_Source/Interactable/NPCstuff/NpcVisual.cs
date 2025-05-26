using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Interactable.NPCstuff
{
    public class NpcVisual
    {
        private readonly float _bubbleStayAfterDialogueDuration;
        private readonly NpcDialogueBubble _npcDialogueBubble;
        
        public NpcVisual(NpcDialogueBubble npcDialogueBubble, float stayDur)
        {
            _npcDialogueBubble = npcDialogueBubble;
            _bubbleStayAfterDialogueDuration = stayDur;

            _npcDialogueBubble.OnDialogueFinished += HideBubble;
        }
        public void CleanUp()
        {
            _npcDialogueBubble.OnDialogueFinished -= HideBubble;
        }
        private void HideBubble()
        {
            HideBubbleAsync(CancellationToken.None).Forget();
        }
        private async UniTask HideBubbleAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_bubbleStayAfterDialogueDuration), cancellationToken: token);
            await _npcDialogueBubble.Disappear(token);
        }
    }
}