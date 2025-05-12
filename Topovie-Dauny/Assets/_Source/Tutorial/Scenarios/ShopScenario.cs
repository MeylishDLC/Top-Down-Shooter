using System;
using System.Threading;
using Cinemachine;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using GameEnvironment;
using GameEnvironment.ShopLogic;
using GameEnvironment.ShopLogic.UIShop;
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
        
        [Header("Dialogues")]
        [SerializeField] private TextAsset dialogueOnShowShop;
        [SerializeField] private TextAsset dialogueOnCloseShop;
        
        [Header("Cameras")]
        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera shopCamera;
        
        [Header("Shop Components")]
        [SerializeField] private ShopTrigger shopTrigger;
        [SerializeField] private Shop shop;
        
        [Header("Timings Settings")]
        [SerializeField] private float timeBeforeCameraSwitch = 2;
        [SerializeField] private float timeBeforeDialogueAppear = 1f;
        [SerializeField] private float timeAfterDialogueEnd = 0.3f;
        [SerializeField] private float cameraSwitchBackTime = 3;
        [SerializeField] private float timeAfterShopClosed = 0.2f;

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
            shopTrigger.gameObject.SetActive(true);
            shop.OnShopClosed += ShowFinalDialogue;
            
            await UniTask.Delay(TimeSpan.FromSeconds(cameraSwitchBackTime), cancellationToken: token);
        }

        private void ShowFinalDialogue()
        {
            shop.OnShopClosed -= ShowFinalDialogue;
            ShowFinalDialogueAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask ShowFinalDialogueAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeAfterShopClosed), cancellationToken: token);
            _dialogueManager.EnterDialogueMode(dialogueOnCloseShop);
            _dialogueManager.OnDialogueEnded += FinishScenario;
        }
        private void FinishScenario()
        {
            _dialogueManager.OnDialogueEnded -= FinishScenario;
            OnEndScenario?.Invoke();
        }
        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
    }
}