using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.Visual
{
    public class BossLeoVisual
    {
        private static readonly int IsHit = Animator.StringToHash("isHit");
        private readonly Animator _animator;
        private readonly float _hurtDuration;
        private readonly CancellationToken _ct;
        
        private bool _isAnimationPlaying;
        public BossLeoVisual(Animator animator, float hurtDuration,
            CancellationToken ct)
        {
            _animator = animator;
            _hurtDuration = hurtDuration;
            _ct = ct;
        }
        public void PlayHurtAnimation()
        {
            if (_isAnimationPlaying)
            {
                return;
            }
            PlayHurtAnimationAsync(_ct).Forget();
        }
        private async UniTask PlayHurtAnimationAsync(CancellationToken token)
        {
            _isAnimationPlaying = true;
            _animator.SetTrigger(IsHit);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_hurtDuration), cancellationToken: token);
            _isAnimationPlaying = false;
        }
    }
}