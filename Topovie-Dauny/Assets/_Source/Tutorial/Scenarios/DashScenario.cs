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
    public class DashScenario: ITutorialScenarioControls
    {
        public event Action OnEndScenario;
        [SerializeField] private Image dashIndicator;
        [SerializeField] private float timeBeforeDashIndicatorAppear = 2;
        [SerializeField] private float dashIndicatorDisappearTime = 2;
        
        private InputListener _inputListener;
        private CancellationTokenSource _cancellationToken;
        public void SetupScenario(InputListener listener)
        {
            _inputListener = listener;
            _cancellationToken = new CancellationTokenSource();
            dashIndicator.gameObject.SetActive(false);
        }
        public void PerformScenario()
        {
            if (!_inputListener)
            {
                throw new Exception("Scenario hasn't been setup. Set it up through SetupScenario.");
            }
            ReadDashInputAsync(_cancellationToken.Token).Forget();
        }
        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
        
        private async UniTask ReadDashInputAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeDashIndicatorAppear), cancellationToken: token);
            dashIndicator.gameObject.SetActive(true);
            
            _inputListener.SetInput(false, true);
            _inputListener.SetWalking(true);
            
            _inputListener.OnRollPressed += OnDashPressed;
        }
        private void OnDashPressed()
        {
            _inputListener.OnRollPressed -= OnDashPressed;
            OnDashPressedAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask OnDashPressedAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(dashIndicatorDisappearTime), cancellationToken: token);
            dashIndicator.gameObject.SetActive(false);
            OnEndScenario?.Invoke();
        }
    }
}