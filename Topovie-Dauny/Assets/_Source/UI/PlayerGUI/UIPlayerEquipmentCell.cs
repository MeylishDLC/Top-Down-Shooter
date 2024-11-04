using System;
using System.Globalization;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Player.PlayerAbilities;
using TMPro;
using UI.Core;
using UI.UIShop;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private CustomCursor customCursor;
        [SerializeField] private Slider abilityChargeSlider;

        private bool _canUseAbility = true;
        
        private Color _initialColor;
        private float _remainingTime;
        private float _holdStartTime;
        private bool _isHoldingKey;
        private CancellationTokenSource _cancelCooldownCts = new();
        private InputListener _inputListener;
        
        //todo split logic from displaying
        [Inject]
        public void Construct(InputListener inputListener)
        {
            _inputListener = inputListener;
            _inputListener.OnUseAbilityPressed += UseAbility;
            _inputListener.OnUseAbilityReleased += OnAbilityReleased;
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
            StartCooldown();
        }
        private async UniTask UseAbilityOnKeyHold(CancellationToken token)
        {
            _holdStartTime = Time.time;
            _isHoldingKey = true;
            ResetSliderValue();
            customCursor.SetCrosshair(true);
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
                    StartCooldown();

                    ResetCursor();
                    ResetSliderValue();
                    _isHoldingKey = false;
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
        private void StartCooldown()
        {
            var cooldown = (float)CurrentAbility.CooldownMilliseconds / 1000;
            ShowCooldownStarted(cooldown);
            StartCooldownTracking(cooldown, _cancelCooldownCts.Token).Forget();
        }
        private void ShowCooldownStarted(float cooldown)
        {
            abilityImage.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, transparencyOnCooldown);
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = Mathf.CeilToInt(cooldown).ToString();
        }
        private async UniTask StartCooldownTracking(float cooldown, CancellationToken token)
        {
            _remainingTime = cooldown;

            while (_remainingTime > 0 && !token.IsCancellationRequested)
            {
                cooldownText.text = Mathf.CeilToInt(_remainingTime).ToString();

                await UniTask.Yield(PlayerLoopTiming.Update);

                _remainingTime -= Time.deltaTime;
            }

            ShowCooldownEnded();
        }
        private void CancelCooldownTracking()
        {
            _cancelCooldownCts?.Cancel();
            _cancelCooldownCts?.Dispose();
            _cancelCooldownCts = new CancellationTokenSource();
        }
        private void ResetSliderValue()
        {
            abilityChargeSlider.gameObject.SetActive(false);
            abilityChargeSlider.value = 0;
        }
        private void ResetCursor()
        {
            customCursor.SetCrosshair(false);
        }
        private void ShowCooldownEnded()
        {
            _canUseAbility = true;

            _remainingTime = 0;
            abilityImage.color = _initialColor;
            cooldownText.gameObject.SetActive(false);
            
            customCursor.SetCrosshair(false); 
            ResetSliderValue();
        }
        private void SwitchAbility(int abilityCellIndex, Ability newAbility)
        {
            if (abilityCellIndex == cellIndex)
            {
                CurrentAbility = newAbility;
                abilityImage.sprite = CurrentAbility.AbilityImage;
                
                CancelCooldownTracking();
            }
        }
        
    }
}