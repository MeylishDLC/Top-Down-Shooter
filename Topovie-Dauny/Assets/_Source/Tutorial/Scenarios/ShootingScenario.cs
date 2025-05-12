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
    public class ShootingScenario: ITutorialScenarioControls
    {
        public event Action OnEndScenario;
        
        [SerializeField] private Image attackIndicator;
        [SerializeField] private float timeBeforeAttackIndicatorAppear = 2;
        [SerializeField] private float attackIndicatorDisappearTime = 2;
        
        private InputListener _inputListener;
        private CancellationTokenSource _cancellationToken;
        
        public void SetupScenario(InputListener listener)
        {
            _inputListener = listener;
            _cancellationToken = new CancellationTokenSource();
            attackIndicator.gameObject.SetActive(false);
        }
        
        public void PerformScenario()
        {
            if (!_inputListener)
            {
                throw new Exception("Scenario hasn't been setup. Set it up through SetupScenario.");
            }
            ReadShootInput(_cancellationToken.Token).Forget();
        }

        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
        
        private async UniTask ReadShootInput(CancellationToken token)
        {
            _inputListener.SetInput(false, true);
            _inputListener.SetWalking(true);
            _inputListener.SetFiringAbility(true);
            
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeAttackIndicatorAppear), cancellationToken: token);
            attackIndicator.gameObject.SetActive(true);
            _inputListener.OnFirePressed += GetFirePressed;
        }
        private void GetFirePressed()
        {
            _inputListener.OnFirePressed -= GetFirePressed;
            OnFirePressedAsync(CancellationToken.None).Forget();
        }
        private async UniTask OnFirePressedAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(attackIndicatorDisappearTime), cancellationToken: token);
            attackIndicator.gameObject.SetActive(false);
            OnEndScenario?.Invoke();
        }
    }
}