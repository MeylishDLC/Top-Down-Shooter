using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player.PlayerAbilities
{
    public class AbilityTimer
    {
        public event Action OnCooldownEnded;
        public float RemainingTime {get; private set;}

        private Ability _currentAbility;
        private CancellationTokenSource _cancelCooldownCts = new();
        public AbilityTimer(Ability ability)
        {
            _currentAbility = ability;
        }
        public void SetCurrentAbility(Ability ability)
        {
            CancelCooldownTracking();
            _currentAbility = ability;
        }
        public void StartCooldown(float cooldown)
        {
            StartCooldownTracking(cooldown, _cancelCooldownCts.Token).Forget();
        }
        public void Expose()
        {
            _cancelCooldownCts?.Cancel();
            _cancelCooldownCts?.Dispose();
        }
        private async UniTask StartCooldownTracking(float cooldown, CancellationToken token)
        {
            RemainingTime = cooldown;

            while (RemainingTime > 0 && !token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);

                RemainingTime -= Time.deltaTime;
            }

            OnCooldownEnded?.Invoke();
        }
        private void CancelCooldownTracking()
        {
            _cancelCooldownCts?.Cancel();
            _cancelCooldownCts?.Dispose();
            _cancelCooldownCts = new CancellationTokenSource();
        }
    }
}