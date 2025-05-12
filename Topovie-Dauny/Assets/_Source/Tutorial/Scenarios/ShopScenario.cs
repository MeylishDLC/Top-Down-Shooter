using System;
using System.Threading;
using Cinemachine;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using Tutorial.Scenarios.ScenariosTypes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Tutorial.Scenarios
{
    [System.Serializable]
    public class ShopScenario: ITutorialScenarioWithDialogue
    {
        public event Action OnEndScenario;
        
        [SerializeField] private TextAsset dialogueOnShowShop;
        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera shopCamera;
        [SerializeField] private float timeBeforeCameraSwitch = 2;
        [SerializeField] private float timeBeforeDialogueAppear = 1f;
        [SerializeField] private float timeAfterDialogueEnd = 0.3f;
        [SerializeField] private float cameraSwitchBackTime = 3;

        private int _mainCamPriority;
        
        private InputListener _inputListener;
        private DialogueManager _dialogueManager;
        private CancellationTokenSource _cancellationToken;
        
        public void SetupScenario(InputListener listener, DialogueManager dialogueManager)
        {
            _inputListener = listener;
            _dialogueManager = dialogueManager;
            _cancellationToken = new CancellationTokenSource();
            _mainCamPriority = mainCamera.Priority;
        }
        public void PerformScenario()
        {
            if (!_inputListener || _dialogueManager == null)
            {
                throw new Exception("Scenario hasn't been setup. Set it up through SetupScenario.");
            }
            SwitchCamToShopAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask SwitchCamToShopAsync(CancellationToken token)
        {
            _inputListener.SetInput(false, true);
            
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeCameraSwitch), cancellationToken: token);
            shopCamera.Priority = _mainCamPriority++;
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeDialogueAppear), cancellationToken: token);
            
            PlayDialogue();
        }
        private void PlayDialogue()
        {
            _dialogueManager.EnterDialogueMode(dialogueOnShowShop);
            _dialogueManager.OnDialogueEnded += SwitchCamToMain;
        }

        private void SwitchCamToMain()
        {
            _dialogueManager.OnDialogueEnded -= SwitchCamToMain;
            SwitchCamToMainAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask SwitchCamToMainAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeAfterDialogueEnd), cancellationToken: token);
            shopCamera.Priority = _mainCamPriority - 2;

            _inputListener.SetInput(true);
            
            await UniTask.Delay(TimeSpan.FromSeconds(cameraSwitchBackTime), cancellationToken: token);
            OnEndScenario?.Invoke();
        }
        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
    }
}