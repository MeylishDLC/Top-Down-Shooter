using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Tutorial.Scenarios.ScenariosTypes;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Tutorial.Scenarios
{
    [System.Serializable]
    public class WalkScenario: ITutorialScenarioControls
    {
        public event Action OnEndScenario;
        
        [SerializeField] private Image wasdIndicator;
        [SerializeField] private float timeBeforeWasdIndicatorAppear = 1;
        [SerializeField] private float wasdIndicatorDisappearTime = 2;
        
        private InputListener _inputListener;
        private CancellationTokenSource _cancellationToken;

        public void SetupScenario(InputListener listener)
        {
            _inputListener = listener;
            _cancellationToken = new CancellationTokenSource();
            wasdIndicator.gameObject.SetActive(false);
        }
        
        public void PerformScenario()
        {
            if (!_inputListener)
            {
                throw new Exception("Scenario hasn't been setup. Set it up through SetupScenario.");
            }
            StartReadingWalkInputAsync(_cancellationToken.Token).ContinueWith
                (() => OnEndScenario?.Invoke()).Forget();
        }

        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
        private async UniTask StartReadingWalkInputAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeWasdIndicatorAppear), cancellationToken: token);
            wasdIndicator.gameObject.SetActive(true);
            
            await ReadWalkInputAsync(token);
            await UniTask.Delay(TimeSpan.FromSeconds(wasdIndicatorDisappearTime), cancellationToken: token);
            wasdIndicator.gameObject.SetActive(false);
        }
        private async UniTask ReadWalkInputAsync(CancellationToken token)
        {
            try
            {
                while (_inputListener.GetMovementValue() == Vector2.zero)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
    }
}