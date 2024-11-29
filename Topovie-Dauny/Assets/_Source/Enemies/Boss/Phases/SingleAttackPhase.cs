using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Enemies.Boss.BossAttacks;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies.Boss.Phases
{
    public class SingleAttackPhase: IBossPhase
    {
        public event Action<PhaseState> OnPhaseStateChanged;
        public BasePhaseConfig PhaseConfig { get; }

        private readonly List<IBossAttack> _bossAttacks;
        private readonly float _minVulnerabilityDur;
        private readonly float _maxVulnerabilityDur;
        
        private PhaseState _currentState = PhaseState.Attack;
        private int _currentAttackIndex;
        private readonly CancellationTokenSource _stopPhaseCts = new();
        
        public SingleAttackPhase(BasePhaseConfig config, List<IBossAttack> bossAttacks)
        {
            PhaseConfig = config;
            _bossAttacks = bossAttacks;
            _minVulnerabilityDur = config.MinVulnerabilityDuration;
            _maxVulnerabilityDur = config.MaxVulnerabilityDuration;
        }
        
        public void StartPhase()
        {
            StartPhaseAsync(_stopPhaseCts.Token).Forget();
        }

        public void FinishPhase()
        {
            Debug.Log("Finish Phase");
            _stopPhaseCts.Cancel();
            _stopPhaseCts.Dispose();
        }
        private async UniTask StartPhaseAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await MoveToAttackStateAsync(token);
                    await MoveToVulnerabilityStateAsync(token);
                }  
            }
            catch (TaskCanceledException)
            {
                //
            }
        }
        private async UniTask MoveToAttackStateAsync(CancellationToken token)
        {
            Debug.Log("Attack");
            _currentState = PhaseState.Attack;
            OnPhaseStateChanged?.Invoke(_currentState);

            if (_currentAttackIndex >= _bossAttacks.Count)
            {
                _currentAttackIndex = 0;
            }
            await _bossAttacks[_currentAttackIndex].TriggerAttack(token);
            _currentAttackIndex++;
        }

        private async UniTask MoveToVulnerabilityStateAsync(CancellationToken token)
        {
            _currentState = PhaseState.Vulnerability;
            OnPhaseStateChanged?.Invoke(_currentState);
            Debug.Log("Vulnerability");
  
            var duration = GetRandomVulnerabilityDuration();
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
        }

        private float GetRandomVulnerabilityDuration()
        {
            return UnityEngine.Random.Range(_minVulnerabilityDur, _maxVulnerabilityDur);
        }

    }
}