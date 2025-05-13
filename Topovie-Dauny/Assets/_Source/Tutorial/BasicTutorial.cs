using System;
using System.Collections.Generic;
using System.Threading;
using Core.InputSystem;
using Core.SceneManagement;
using Core.Utilities;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using GameEnvironment;
using GameEnvironment.ShopLogic;
using Tutorial.Scenarios.ScenariosTypes;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Tutorial
{
    public class BasicTutorial: MonoBehaviour
    { 
        [Header("Dialogues")] 
        [SerializeField] private TextAsset dialogueOnTutorialStart;
        [SerializeField] private TextAsset dialogueOnTutorialEnd;

        [Header("Interactable Components")]
        [SerializeField] private ShopTrigger shopTrigger;
        [SerializeField] private PortalChargerTrigger portalTrigger;
        [SerializeField] private ChargerZone chargeZone;
        
        [Header("Tutorial Scenarios")]
        [SerializeReference]
        public List<ITutorialScenario> scenarios = new();

        private CancellationToken _ctOnDestroy;
        private InputListener _inputListener;
        private DialogueManager _dialogueManager;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(InputListener inputListener, DialogueManager dialogueManager, SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _dialogueManager = dialogueManager;
            _inputListener = inputListener;
        }
        private void Start()
        {
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();
        }
        public void EnableTutorial()
        {
            if (_sceneLoader.CurrentSceneIndex == _sceneLoader.LastSceneIndex)
            {
                return;
            }
            portalTrigger.gameObject.SetActive(false);
            shopTrigger.gameObject.SetActive(false);
            chargeZone.gameObject.SetActive(false);
            
            _inputListener.SetInput(false, true);
            
            _dialogueManager.EnterDialogueMode(dialogueOnTutorialStart);
            _dialogueManager.OnDialogueEnded += StartTutorial;
        }
        private void StartTutorial()
        {
            _inputListener.SetInput(false, true);
            _inputListener.SetWalking(true);
            _dialogueManager.OnDialogueEnded -= StartTutorial;

            PlayScenarios(_ctOnDestroy).Forget();
        }
        private async UniTask PlayScenarios(CancellationToken token)
        {
            await StartScenariosSequenceAsync(token);
            EndTutorial();
        }
        private async UniTask StartScenariosSequenceAsync(CancellationToken token)
        {
            foreach (var scenario in scenarios)
            {
                if (scenario is ITutorialScenarioControls scenarioControls)
                {
                    scenarioControls.SetupScenario(_inputListener);
                }
                else if (scenario is ITutorialScenarioWithDialogue scenarioWithDialogue)
                {
                    scenarioWithDialogue.SetupScenario(_inputListener, _dialogueManager);
                }
                scenario.PerformScenario();
                await EventAwaiter.AwaitEvent(
                    h => scenario.OnEndScenario += h,
                    h => scenario.OnEndScenario -= h);
                scenario.CleanUp();
            }
        }
        
        private void EndTutorial()
        {
            shopTrigger.gameObject.SetActive(true);
            portalTrigger.gameObject.SetActive(true);
            chargeZone.gameObject.SetActive(true);

            _inputListener.SetInteract(true);
            _inputListener.SetInput(true);
            _dialogueManager.EnterDialogueMode(dialogueOnTutorialEnd);
        }
    }
}