using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Tutorial.Scenarios.ScenariosTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.Scenarios
{
    [System.Serializable]
    public class AbilityScenario: ITutorialScenarioControls
    {
        public event Action OnEndScenario;
        
        [SerializeField] private Image abilitiesIndicator;
        [SerializeField] private float timeBeforeAbilitiesIndicatorAppear = 2;
        [SerializeField] private float abilityIndicatorDisappearTime = 1;
        
        private CancellationTokenSource _cancellationToken;
        private InputListener _inputListener;
        
        public void SetupScenario(InputListener listener)
        {
            _cancellationToken = new CancellationTokenSource();
            _inputListener = listener;
        }
        
        public void PerformScenario()
        {
            if (!_inputListener)
            {
                throw new Exception("Scenario hasn't been setup. Set it up through SetupScenario.");
            }
            ReadUseAbilityInput(_cancellationToken.Token).Forget();
        }
        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
        private async UniTask ReadUseAbilityInput(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeAbilitiesIndicatorAppear), cancellationToken: token);
            abilitiesIndicator.gameObject.SetActive(true);
            
            _inputListener.SetInput(false, true);
            _inputListener.SetWalking(true);
            _inputListener.SetFiringAbility(true);
            _inputListener.SetUseAbility(true);
            
            _inputListener.OnUseAbilityPressed += OnAbilityUsed;
        }
        private void OnAbilityUsed(int _)
        {
            _inputListener.OnUseAbilityPressed -= OnAbilityUsed;
            OnAbilityUsedAsync(CancellationToken.None).Forget();
        }
        private async UniTask OnAbilityUsedAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(abilityIndicatorDisappearTime), cancellationToken: token);
            abilitiesIndicator.gameObject.SetActive(false);
            OnEndScenario?.Invoke();
        }
    }
}