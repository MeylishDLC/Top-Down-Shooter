using System;
using System.Linq;
using System.Threading;
using Core.InputSystem;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using Player.PlayerAbilities;
using SoundSystem;
using SoundSystem.DialogueSoundSO;
using TMPro;
using UI.UIShop.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.UIShop
{
    public class Shop: MonoBehaviour
    {
        [field:SerializeField] public GameObject EquipScreen { get; private set; } 
        [field:SerializeField] public InfoPanel InfoPanel { get; private set; }
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject playerGUI;
        [SerializeField] private Button closeButton;

        [Header("Vet Dialogue")] 
        [SerializeField] private LevelChargesHandler levelChargesHandler;
        [SerializeField] private VetDialogueConfig dialogueConfig;
        [SerializeField] private DialogueAudioInfoSO dialogueAudioSO;
        [SerializeField] private TMP_Text vetDialogueText;
        [SerializeField] private int typeSpeedMilliseconds;
        [SerializeField] private int delayBeforeDialogueChangeMilliseconds;
        [SerializeField] private int delayBeforeShopClosingMillisecons;
        [SerializeField] private PlayerCellsInShop playerCellsInShop;

        private AudioManager _audioManager;
        private StatesChanger _statesChanger;
        private InputListener _inputListener;
        private ShopDialogue _shopDialogue;
        private bool _isTyping;
        private bool _isClosing;
        private CancellationTokenSource _stopTypingCts = new();

        [Inject]
        public void Construct(InputListener inputListener, StatesChanger statesChanger, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _statesChanger = statesChanger;
            _inputListener = inputListener;
        }
        private void Start()
        {
            shopUI.SetActive(false);
            closeButton.onClick.AddListener(CloseShop);
            playerCellsInShop.OnAbilityChanged += ChangeDialogue;
            _shopDialogue = new ShopDialogue(vetDialogueText, typeSpeedMilliseconds, dialogueAudioSO, _audioManager);
        }
        //todo fix shooting in shop after dialogue
        private void OnDestroy()
        {
            playerCellsInShop.OnAbilityChanged -= ChangeDialogue;
            _stopTypingCts?.Dispose();
        }
        public void OpenShop()
        {
            if (!IsShopOpen())
            {
                _audioManager.PlayOneShot(_audioManager.FMODEvents.ShopEnterSound);
                OpenShopAsync(_stopTypingCts.Token).Forget();
            }
        }
        public bool IsShopOpen()
        {
            return shopUI.activeSelf;
        }
        private void CloseShop()
        {
            if (IsShopOpen() && !_isClosing)
            {
                _isClosing = true;
                CloseShopAsync(_stopTypingCts.Token).Forget();
            }
        }
        private async UniTask OpenShopAsync(CancellationToken token)
        {
            DisableInput();
            shopUI.SetActive(true);
            playerGUI.SetActive(false);
            vetDialogueText.text = "";

            _isTyping = true;
            var currentDialoguePack = GetCurrentDialoguePack();
            await _shopDialogue.TypeDialogueAsync(currentDialoguePack.Greeting, token);
            _isTyping = false;
        }

        private async UniTask CloseShopAsync(CancellationToken token)
        {
            EnableInput();
            _isTyping = true;
            var currentDialoguePack = GetCurrentDialoguePack();
            await _shopDialogue.TypeDialogueAsync(currentDialoguePack.Goodbye, token);
            _isTyping = false;
            await UniTask.Delay(delayBeforeShopClosingMillisecons, cancellationToken: token);
            
            shopUI.SetActive(false);
            playerGUI.SetActive(true);
            _isClosing = false;
        }
        private void DisableInput()
        {
            _inputListener.SetFiringAbility(false);
            _inputListener.SetUseAbility(false);
        }
        private void EnableInput()
        {
            _inputListener.SetFiringAbility(true);
            _inputListener.SetUseAbility(true);
        }
        private void ChangeDialogue(int _, Ability ability)
        {
            if (_isTyping)
            {
                _stopTypingCts?.Cancel();
                _stopTypingCts?.Dispose();
                _stopTypingCts = new();
                _isTyping = false;
            }
            ChangeDialogueAsync(ability, _stopTypingCts.Token).Forget();
        }
        private async UniTask ChangeDialogueAsync(Ability ability, CancellationToken token)
        {
            vetDialogueText.text = "";
            _isTyping = true;
            await UniTask.Delay(delayBeforeDialogueChangeMilliseconds, cancellationToken: token);
            await _shopDialogue.TypeDialogueAsync(GetDialogueForAbility(ability), token);
            _isTyping = false;
        }

        private string GetDialogueForAbility(Ability ability)
        {
            var abilityDialogues = GetCurrentDialoguePack().AbilityDialoguePairs;
            foreach (var pair in abilityDialogues)
            {
                var type = pair.AbilityType.Type;
                if (type.IsAssignableFrom(ability.GetType()))
                {
                    return pair.Dialogue;
                }
            }
            throw new Exception($"Dialogue for ability type {ability.GetType()} not found. Make sure to assign it in config");
        }
        private DialoguePack GetCurrentDialoguePack()
        {
            if (levelChargesHandler == null)
            {
                Debug.LogWarning("Level Charges Handler is not initialized. Charges passed is always zero.");
                return dialogueConfig.ChargesDialoguePacks[0];
            }
            var dialogueIndex = levelChargesHandler.ChargesPassed;

            if (dialogueIndex >= dialogueConfig.ChargesDialoguePacks.Length)
            {
                dialogueIndex = dialogueConfig.ChargesDialoguePacks.Length - 1;
            }
            return dialogueConfig.ChargesDialoguePacks[dialogueIndex];
        }
    }
}