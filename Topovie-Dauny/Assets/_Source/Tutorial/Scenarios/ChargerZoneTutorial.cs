using System;
using System.Threading;
using Cinemachine;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using Tutorial.Scenarios.ScenariosTypes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace Tutorial.Scenarios
{
    [System.Serializable]
    public class ChargerZoneTutorial: ITutorialScenarioWithDialogue
    {
        public event Action OnEndScenario;
        
        [SerializeField] private TutorialChargerZone chargerZone;
        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera chargerZoneCamera;
        
        [Header("Dialogues")]
        [SerializeField] private TextAsset showChargerZoneDialogue;
        [SerializeField] private TextAsset explainChargerZoneDialogue;
        
        [Header("Timings Settings")]
        [SerializeField] private float timeBeforeCameraSwitch = 2f;
        [SerializeField] private float timeBeforeShowZoneDialogue = 1f;
        [SerializeField] private float timeAfterDialogue = 1f;
        [SerializeField] private float timeBeforeExplainZoneDialogue = 2.5f;
        [SerializeField] private float timeBeforeChargerZoneDisable = 1f;
        [SerializeField] private float timeAfterChargerZoneDisable = 1f;
        
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
            chargerZone.gameObject.SetActive(false);
        }
        public void PerformScenario()
        {
            if (!_inputListener || _dialogueManager == null)
            {
                throw new Exception("Scenario hasn't been setup. Set it up through SetupScenario.");
            }
            
            ShowChargerZoneAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask ShowChargerZoneAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeCameraSwitch), cancellationToken: token);
            _inputListener.SetInput(false, true);
            chargerZoneCamera.Priority = _mainCamPriority++;
            
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeShowZoneDialogue), cancellationToken: token);
            PlayShowChargerZoneDialogue();
        }
        private void PlayShowChargerZoneDialogue()
        {
            _dialogueManager.OnDialogueEnded += SwitchCameraBack;
            _dialogueManager.EnterDialogueMode(showChargerZoneDialogue);
        }
        private void SwitchCameraBack()
        {
            _dialogueManager.OnDialogueEnded -= SwitchCameraBack;
            SwitchCameraBackAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask SwitchCameraBackAsync(CancellationToken token)
        {
            //switch cam back to player
            await UniTask.Delay(TimeSpan.FromSeconds(timeAfterDialogue), cancellationToken: token);
            chargerZoneCamera.Priority = _mainCamPriority - 2;
            
            _inputListener.SetInput(true);
            chargerZone.gameObject.SetActive(true);
            //wait until player enters the zone
            WaitForPlayerInZoneAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask WaitForPlayerInZoneAsync(CancellationToken token)
        {
            while (!chargerZone.IsPlayerInRange)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            await chargerZone.Fill(1f);
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeExplainZoneDialogue), cancellationToken: token);
            StartExplainZoneDialogue();
        }

        private void StartExplainZoneDialogue()
        {
            _dialogueManager.OnDialogueEnded += DisableTutorialChargerZone;
            _dialogueManager.EnterDialogueMode(explainChargerZoneDialogue);
        }
        private void DisableTutorialChargerZone()
        {
            _dialogueManager.OnDialogueEnded -= DisableTutorialChargerZone;
            DisableTutorialChargerZoneAsync(_cancellationToken.Token).Forget();
        }
        private async UniTask DisableTutorialChargerZoneAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeChargerZoneDisable), cancellationToken: token);
            await chargerZone.Fill(0f);
            chargerZone.gameObject.SetActive(false);
            Object.Destroy(chargerZoneCamera.gameObject);
            await UniTask.Delay(TimeSpan.FromSeconds(timeAfterChargerZoneDisable), cancellationToken: token);
            OnEndScenario?.Invoke();
        }
        public void CleanUp()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
    }
}