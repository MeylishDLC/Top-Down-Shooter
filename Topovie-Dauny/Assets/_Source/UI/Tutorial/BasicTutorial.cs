using System;
using System.Threading;
using Core.InputSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using GameEnvironment;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Tutorial
{
    public class BasicTutorial: MonoBehaviour
    {
        [SerializeField] private Image wasdIndicator;
        [SerializeField] private Image attackIndicator;
        [SerializeField] private Image abilitiesIndicator;

        [Header("Dialogues")] 
        [SerializeField] private TextAsset dialogueOnTutorialStart;
        [SerializeField] private TextAsset dialogueOnTutorialEnd;

        [Header("Interactable Components")]
        [SerializeField] private ShopTrigger shopTrigger;
        [SerializeField] private PortalChargerTrigger portalTrigger;
        
        [Header("Timing Settings")]
        [Header("WASD Indicator Settings")]
        [SerializeField] private float timeBeforeWasdIndicatorAppear;
        [SerializeField] private float wasdIndicatorDisappearTime;

        [Header("Attack Indicator Settings")]
        [SerializeField] private float timeBeforeAttackIndicatorAppear;
        [SerializeField] private float attackIndicatorDisappearTime;
        
        [Header("Abilities Indicator Settings")]
        [SerializeField] private float timeBeforeAbilitiesIndicatorAppear;
        [SerializeField] private float abilityIndicatorDisappearTime;
        
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
            wasdIndicator.gameObject.SetActive(false);
            abilitiesIndicator.gameObject.SetActive(false);
            abilitiesIndicator.gameObject.SetActive(false);
        }
        public void EnableTutorial()
        {
            if (_sceneLoader.CurrentSceneIndex == _sceneLoader.LastSceneIndex)
            {
                return;
            }
            portalTrigger.enabled = false;
            shopTrigger.enabled = false;
            
            _inputListener.SetInput(false, true);
            _dialogueManager.EnterDialogueMode(dialogueOnTutorialStart);
            _dialogueManager.OnDialogueEnded += StartTutorial;
        }
        private void StartTutorial()
        {
            _inputListener.SetInput(true);
            _dialogueManager.OnDialogueEnded -= StartTutorial;
            _inputListener.SetFiringAbility(false);
            _inputListener.SetUseAbility(false);
            _inputListener.SetInteract(false);
            ReadWalkInput(CancellationToken.None).ContinueWith(() => ReadShootInput(CancellationToken.None)).Forget();
        }
        private async UniTask ReadWalkInput(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeWasdIndicatorAppear), cancellationToken: token);
            wasdIndicator.gameObject.SetActive(true);
            
            await ReadWalkInputAsync(token);
            await UniTask.Delay(TimeSpan.FromSeconds(wasdIndicatorDisappearTime), cancellationToken: token);
            wasdIndicator.gameObject.SetActive(false);
        }
        private async UniTask ReadWalkInputAsync(CancellationToken token)
        {
            while (_inputListener.GetMovementValue() == Vector2.zero)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private async UniTask ReadShootInput(CancellationToken token)
        {
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
            await ReadUseAbilityInput(token);
        }
        private async UniTask ReadUseAbilityInput(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeAbilitiesIndicatorAppear), cancellationToken: token);
            abilitiesIndicator.gameObject.SetActive(true);
            
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
            
            //todo enter dialogue mode
            EndTutorial();
        }
        private void EndTutorial()
        {
            portalTrigger.enabled = true;
            shopTrigger.enabled = true;
            _inputListener.SetInteract(true);
            _dialogueManager.EnterDialogueMode(dialogueOnTutorialEnd);
        }
    }
}