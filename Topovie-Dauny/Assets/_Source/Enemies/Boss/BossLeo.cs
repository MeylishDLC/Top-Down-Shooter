using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using Enemies.Boss.Phases;
using UnityEngine;
using Zenject;

namespace Enemies.Boss
{
    public class BossLeo : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<BossPhase, TextAsset> phaseDialoguePair;
        [SerializeField] private BossHealth bossHealth;
        
        private int _currentPhaseIndex;
        private CancellationToken _destroyCancellationToken;
        private StatesChanger _statesChanger;
        private DialogueManager _dialogueManager;
        
        [Inject]
        public void Construct(StatesChanger statesChanger, DialogueManager dialogueManager)
        {
            _statesChanger = statesChanger;
            _dialogueManager = dialogueManager;
        }
        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            bossHealth.OnPhaseFinished += EndPhase;
            StartFight();
        }

        private void OnDestroy()
        {
            bossHealth.OnPhaseFinished -= EndPhase;
        }

        private void StartFight()
        {
            _statesChanger.ChangeState(GameStates.Fight);
            StartPhase();
        }

        private void EndPhase()
        {
            phaseDialoguePair.Keys.ElementAt(_currentPhaseIndex).FinishPhase();
            
            PlayDialogue();
        }
        private void StartPhase()
        {
            _dialogueManager.OnDialogueEnded -= StartPhase;
            
            StartPhaseAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask StartPhaseAsync(CancellationToken token)
        {
            if (_currentPhaseIndex >= phaseDialoguePair.Count)
            {
                Debug.Log("Phases passed");
                return;
            }

            var phase = phaseDialoguePair.Keys.ElementAt(_currentPhaseIndex);
           
            if (_currentPhaseIndex > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(phase.PhaseConfig.AttackTransitionDuration),
                    cancellationToken: token);
            }
            bossHealth.ChangePhase(phase);
            phase.StartPhase();
        }

        private void PlayDialogue()
        {
            _dialogueManager.OnDialogueEnded += StartPhase;
            var dialogue = phaseDialoguePair.Values.ElementAt(_currentPhaseIndex);
            _currentPhaseIndex++;
            if (dialogue is null)
            {
                Debug.LogWarning($"No dialogue for {_currentPhaseIndex} phase}}");
                StartPhase();
                return;
            }
            
            _dialogueManager.EnterDialogueMode(dialogue);
        }
    }
}
