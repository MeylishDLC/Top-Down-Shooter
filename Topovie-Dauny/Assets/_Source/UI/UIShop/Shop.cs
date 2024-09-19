using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerAbilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.UIShop
{
    public class Shop: MonoBehaviour
    {
        public bool ShopIsOpen { get; private set; }
        [field:SerializeField] public GameObject EquipScreen { get; private set; } 
        [field:SerializeField] public InfoPanel InfoPanel { get; private set; }
        
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject playerGUI;
        [SerializeField] private Button closeButton;

        [Header("Vet Dialogue")] 
        [TextArea] [SerializeField] private string speechOnEnterShop;
        [TextArea] [SerializeField] private string speechOnExitShop;
        [SerializeField] private TMP_Text vetDialogueText;
        [SerializeField] private int typeSpeedMilliseconds;
        [SerializeField] private int delayBeforeDialogueChangeMilliseconds;
        [SerializeField] private int delayBeforeShopClosingMillisecons;
        [SerializeField] private PlayerCellsInShop playerCellsInShop;

        private ShopDialogue _shopDialogue;
        private bool _isTyping;
        private CancellationTokenSource _stopTypingCts = new();
        private void Start()
        {
            shopUI.SetActive(false);
            closeButton.onClick.AddListener(CloseShop);
            playerCellsInShop.OnAbilityChanged += ChangeDialogue;
            _shopDialogue = new ShopDialogue(vetDialogueText, typeSpeedMilliseconds);
        }
        private void OnDestroy()
        {
            playerCellsInShop.OnAbilityChanged -= ChangeDialogue;
            _stopTypingCts?.Dispose();
        }
        public void OpenShop()
        {
            if (!ShopIsOpen)
            {
                OpenShopAsync(_stopTypingCts.Token).Forget();
            }
        }
        private void CloseShop()
        {
            CloseShopAsync(_stopTypingCts.Token).Forget();
        }
        private async UniTask OpenShopAsync(CancellationToken token)
        {
            ShopIsOpen = true;
            shopUI.SetActive(true);
            playerGUI.SetActive(false);
            vetDialogueText.text = "";

            _isTyping = true;
            await _shopDialogue.TypeDialogueAsync(speechOnEnterShop, token);
            _isTyping = false;
        }

        private async UniTask CloseShopAsync(CancellationToken token)
        {
            _isTyping = true;
            await _shopDialogue.TypeDialogueAsync(speechOnExitShop, token);
            _isTyping = false;
            await UniTask.Delay(delayBeforeShopClosingMillisecons, cancellationToken: token);
            
            ShopIsOpen = false;
            shopUI.SetActive(false);
            playerGUI.SetActive(true);
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
            await _shopDialogue.TypeDialogueAsync(ability.VetReactionText, token);
            _isTyping = false;
        }
    }
}