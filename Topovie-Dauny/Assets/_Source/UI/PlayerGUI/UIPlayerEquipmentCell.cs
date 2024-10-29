using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerAbilities;
using TMPro;
using UI.Core;
using UI.UIShop;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.PlayerGUI
{
    public class UIPlayerEquipmentCell: MonoBehaviour
    {
        public event Action<Ability> OnUseAbility; 
        public Ability CurrentAbility { get; private set; }

        //todo refactor
        [SerializeField] private Ability initialAbility;
        [SerializeField] private KeyCode keyToUse;
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
        }
        private void Update()
        {
            if (!_canUseAbility)
            {
                return;
            }
            if (!_isHoldingKey)
            {
                ResetSliderValue();
            }
            if (_isHoldingKey && !Input.GetKey(keyToUse))
            {
                _isHoldingKey = false;
                ResetSliderValue();
            }
            
            if (CurrentAbility.AbilityType == AbilityTypes.TapButton)
            {
                UseAbilityOnKeyTap();
            }
            if (CurrentAbility.AbilityType == AbilityTypes.HoldButton)
            {
                UseAbilityOnKeyHold();
            }
        }
        private void UseAbilityOnKeyTap()
        {
            if (Input.GetKeyDown(keyToUse))
            {
                OnUseAbility?.Invoke(CurrentAbility);
                _canUseAbility = false;
                StartCooldown();
            }
        }
        private void UseAbilityOnKeyHold()
        {
            if (Input.GetKeyDown(keyToUse))
            {
                _holdStartTime = Time.time;
                _isHoldingKey = true;
                
                customCursor.SetCrosshair(true);
                abilityChargeSlider.gameObject.SetActive(true);
                abilityChargeSlider.value = 0;
            }
            else if (Input.GetKey(keyToUse) && _isHoldingKey)
            {
                var elapsedTime = Time.time - _holdStartTime;
                var fillAmount = Mathf.Clamp01(elapsedTime / timeToHoldKey);
                abilityChargeSlider.value = fillAmount;

                if (elapsedTime >= timeToHoldKey)
                {
                    OnUseAbility?.Invoke(CurrentAbility);
                    _canUseAbility = false;
                    StartCooldown();

                    customCursor.SetCrosshair(false);
                    _isHoldingKey = false;
                    ResetSliderValue();
                }
            }
            else if (Input.GetKeyUp(keyToUse))
            {
                customCursor.SetCrosshair(false);
                _isHoldingKey = false;
                ResetSliderValue();
            }
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