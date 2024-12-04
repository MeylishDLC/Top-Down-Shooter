using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Player.PlayerAbilities;
using TMPro;
using UI.Core;
using UI.UIShop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.PlayerGUI
{
    public class UIPlayerEquipmentCell: MonoBehaviour
    {
        public event Action<Ability> OnUseAbility; 
        public Ability CurrentAbility { get; private set; }

        [SerializeField] private Ability initialAbility;
        [SerializeField] private int cellIndex; 
        [SerializeField] private PlayerCellsInShop playerCellsInShop;
        [SerializeField] private float timeToHoldKey;
        
        [Header("UI")]
        [SerializeField] private Image abilityImage;
        [Range(0f,1f)][SerializeField] private float transparencyOnCooldown;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private Slider abilityChargeSlider;

        private bool _canUseAbility = true;
        private Color _initialColor;
        private float _holdStartTime;
        private bool _isHoldingKey;
        
        private CancellationTokenSource _cancelUpdateCooldownCts = new();
        private InputListener _inputListener;
        private CustomCursor _customCursor;
        private AbilityTimer _abilityTimer;
        
        [Inject]
        public void Construct(InputListener inputListener, CustomCursor customCursor)
        {
            _inputListener = inputListener;
            _customCursor = customCursor;
            _inputListener.OnUseAbilityPressed += UseAbility;
            _inputListener.OnUseAbilityReleased += OnAbilityReleased;
            _abilityTimer = new AbilityTimer(initialAbility);
            _abilityTimer.OnCooldownEnded += ShowCooldownEnded;
        }
        private void Awake()
        {
            playerCellsInShop.OnAbilityChanged += SwitchAbility;
            _initialColor = abilityImage.color;
            cooldownText.gameObject.SetActive(false);
            CurrentAbility = initialAbility;
            ResetSliderValue();
        }
        private void OnDestroy()
        {
            playerCellsInShop.OnAbilityChanged -= SwitchAbility;
            _inputListener.OnUseAbilityPressed -= UseAbility;
            _inputListener.OnUseAbilityReleased -= OnAbilityReleased;
            _abilityTimer.OnCooldownEnded -= ShowCooldownEnded;

            _cancelUpdateCooldownCts?.Cancel();
            _cancelUpdateCooldownCts?.Dispose();
            _abilityTimer.Expose();
        }
        private void UseAbility(int abilityNumber)
        {
            if (abilityNumber-1 != cellIndex)
            {
                return;
            }
            if (!_canUseAbility)
            {
                return;
            }

            if (CurrentAbility.AbilityType == AbilityTypes.TapButton)
            {
                UseAbilityOnKeyTap();
            }
            else if (CurrentAbility.AbilityType == AbilityTypes.HoldButton)
            {
                UseAbilityOnKeyHold(CancellationToken.None).Forget();
            }
        }
        private void UseAbilityOnKeyTap()
        {
            OnUseAbility?.Invoke(CurrentAbility);
            _canUseAbility = false;
            StartAbilityCooldown();
        }
        private async UniTask UseAbilityOnKeyHold(CancellationToken token)
        {
            _holdStartTime = Time.time;
            _isHoldingKey = true;
            ResetSliderValue();
            _customCursor.SetCrosshair(true);
            abilityChargeSlider.gameObject.SetActive(true);

            while (_isHoldingKey)
            {
                var elapsedTime = Time.time - _holdStartTime;
                var fillAmount = Mathf.Clamp01(elapsedTime / timeToHoldKey);
                abilityChargeSlider.value = fillAmount;

                if (elapsedTime >= timeToHoldKey)
                {
                    OnUseAbility?.Invoke(CurrentAbility);
                    _canUseAbility = false;
                    
                    ResetCursor();
                    ResetSliderValue();
                    _isHoldingKey = false;
                    
                    StartAbilityCooldown();
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private void OnAbilityReleased(int abilityNumber)
        {
            if (abilityNumber-1 != cellIndex)
            {
                return;
            }
            
            _isHoldingKey = false;
            ResetCursor();
            ResetSliderValue();
        }
        private void StartAbilityCooldown()
        {
            var cooldown = (float)CurrentAbility.CooldownMilliseconds / 1000;
            _abilityTimer.StartCooldown(cooldown);
            
            ShowCooldownStarted(cooldown);
            UpdateCooldownText(_cancelUpdateCooldownCts.Token).Forget();
        }
        private void ShowCooldownStarted(float cooldown)
        {
            abilityImage.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, transparencyOnCooldown);
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = Mathf.CeilToInt(cooldown).ToString();
        }
        private async UniTask UpdateCooldownText(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                cooldownText.text = Mathf.CeilToInt(_abilityTimer.RemainingTime).ToString();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private void ResetSliderValue()
        {
            abilityChargeSlider.gameObject.SetActive(false);
            abilityChargeSlider.value = 0;
        }
        private void ResetCursor()
        {
            _customCursor.SetCrosshair(false);
        }
        private void ShowCooldownEnded()
        {
            CancelCooldownTracking();
            
            _canUseAbility = true;

            abilityImage.color = _initialColor;
            cooldownText.gameObject.SetActive(false);
            
            _customCursor.SetCrosshair(false); 
            ResetSliderValue();
        }
        private void SwitchAbility(int abilityCellIndex, Ability newAbility)
        {
            if (abilityCellIndex != cellIndex)
            {
                return;
            }
            
            CurrentAbility = newAbility;
            abilityImage.sprite = CurrentAbility.AbilityImage;
            CancelCooldownTracking();
                
            _abilityTimer.SetCurrentAbility(CurrentAbility);
        }
        private void CancelCooldownTracking()
        {
            _cancelUpdateCooldownCts?.Cancel();
            _cancelUpdateCooldownCts?.Dispose();
            _cancelUpdateCooldownCts = new CancellationTokenSource();
        }
    }
}