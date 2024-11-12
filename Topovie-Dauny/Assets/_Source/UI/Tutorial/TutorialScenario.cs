using System;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using DialogueSystem.TutorialDialogue;
using Player.PlayerControl;
using Player.PlayerControl.GunMovement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Tutorial
{
    public class TutorialScenario : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private DialogueDisplay defaultDialogueDisplay;
        [SerializeField] private TutorialDialogueDisplay tutorialDialogueDisplay;

        [Header("Configuration")] 
        [SerializeField] private GunRotation gunRotation;
        [SerializeField] private TutorialConfig tutorialConfig;

        [Header("UI Elements")] 
        [SerializeField] private Image screenOnWasdExplained;
        [SerializeField] private Image screenOnShootingExplained;
        [SerializeField] private Image screenOnHealthExplained;
        [SerializeField] private Image screenOnAbilitiesExplained;
        [SerializeField] private Image screenOnShopExplained;
        [SerializeField] private Image screenOnChargeExplained;

        private InputListener _inputListener;
        private DialogueManager _dialogueManager;

        private bool _isWasdPressed;
        
        [Inject]
        public void Construct(InputListener inputListener, DialogueManager dialogueManager)
        {
            _inputListener = inputListener;
            _dialogueManager = dialogueManager;
        }
        private void Start()
        {
            SetAllScreensActive(false);
            ShowStartDialogue();
        }
        private void ShowStartDialogue()
        {
            _dialogueManager.OnDialogueEnded += StartTutorial;
            _dialogueManager.EnterDialogueMode(tutorialConfig.OnTutorialStarted);
        }
        private void StartTutorial()
        {
            _dialogueManager.OnDialogueEnded -= StartTutorial;
            
            screenOnWasdExplained.gameObject.SetActive(true);
            gunRotation.enabled = false;

            tutorialDialogueDisplay.OnDialogueEnd += ReadMoveInput;
            tutorialDialogueDisplay.EnterDialogueMode(tutorialConfig.OnWasdExplained);
        }
        private void ReadMoveInput()
        {
            screenOnWasdExplained.gameObject.SetActive(false);
            gunRotation.enabled = true;
            
            tutorialDialogueDisplay.OnDialogueEnd -= ReadMoveInput;
            ReadMoveInputAsync().Forget();
        }
        private async UniTask ReadMoveInputAsync()
        {
            while (_inputListener.GetMovementValue() == Vector2.zero)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(tutorialConfig.TimeOnWasdChecked));
            ProceedToShootingTutorial();
        }
        private void ProceedToShootingTutorial()
        {
            Debug.Log("Wasd tutor passed!!");
            //tutorialDialogueDisplay.OnDialogueEnd += ;
            //tutorialDialogueDisplay.EnterDialogueMode(tutorialConfig.OnShootingExplained);
        }
        private void SetAllScreensActive(bool active)
        {
            screenOnWasdExplained.gameObject.SetActive(active);
            screenOnShootingExplained.gameObject.SetActive(active);
            screenOnHealthExplained.gameObject.SetActive(active);
            screenOnAbilitiesExplained.gameObject.SetActive(active);
            screenOnShopExplained.gameObject.SetActive(active);
            screenOnChargeExplained.gameObject.SetActive(active);
        }
    }
}
