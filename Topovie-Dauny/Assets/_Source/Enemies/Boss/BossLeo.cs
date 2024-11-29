using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Boss.Phases;
using UnityEngine;
using Zenject;

namespace Enemies.Boss
{
    public class BossLeo : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<BossPhase, TextAsset> phaseDialoguePair;
        [SerializeField] private BossHealth bossHealth;
        
        private int _currentPhaseIndex = -1;
        private CancellationToken _destroyCancellationToken;
        
        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            bossHealth.OnPhaseFinished += StartPhase;
            StartFight();
        }

        private void OnDestroy()
        {
            bossHealth.OnPhaseFinished -= StartPhase;
        }

        private void StartFight()
        {
           StartPhase();
        }
        private void StartPhase()
        {
            _currentPhaseIndex++;

            if (_currentPhaseIndex >= phaseDialoguePair.Count)
            {
                Debug.Log("Phases passed");
                return;
            }
            StartPhaseAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask StartPhaseAsync(CancellationToken token)
        {
            var phase = phaseDialoguePair.Keys.ElementAt(_currentPhaseIndex);

            if (_currentPhaseIndex > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(phase.PhaseConfig.AttackTransitionDuration),
                    cancellationToken: token);
            }
            bossHealth.ChangePhase(phase);
            phase.StartPhase();
        }
    }
}
