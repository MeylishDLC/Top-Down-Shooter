using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Enemies.Boss.BossAttacks;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Enemies.Boss.Phases
{
    public class BossPhase: MonoBehaviour
    {
        public event Action<PhaseState> OnPhaseStateChanged;
        [field:SerializeField] public BasePhaseConfig PhaseConfig {get; private set;}

        [SerializeField] private bool randomiseAttacks;
        [BossAttack] [SerializeField] public List<MonoBehaviour> attacks;
        
        private List<IBossAttack> _bossAttacks;
        
        private PhaseState _currentState = PhaseState.Attack;
        private int _currentAttackIndex;
        private readonly CancellationTokenSource _stopPhaseCts = new();
        
        private void Awake()
        {
            _bossAttacks = GetCastedBossAttacksList();
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
            catch (OperationCanceledException)
            {
                //
            }
        }
        private async UniTask MoveToAttackStateAsync(CancellationToken token)
        {
            _currentState = PhaseState.Attack;
            OnPhaseStateChanged?.Invoke(_currentState);

            if (randomiseAttacks)
            {
                var randomAttackIndex = Random.Range(0, attacks.Count);
                await _bossAttacks[randomAttackIndex].TriggerAttack(token);
                return;
            }
            
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
            return Random.Range(PhaseConfig.MinVulnerabilityDuration, PhaseConfig.MaxVulnerabilityDuration);
        }
        private List<IBossAttack> GetCastedBossAttacksList()
        {
            return attacks.Select(x => x.GetComponent<IBossAttack>()).ToList();
        }
    }
}