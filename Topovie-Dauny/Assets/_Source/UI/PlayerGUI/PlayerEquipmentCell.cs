using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerAbilities;
using TMPro;
using UI.UIShop;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.PlayerGUI
{
    public class PlayerEquipmentCell: MonoBehaviour
    {
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
        [SerializeField] private TMP_Text holdKeyText;
        [SerializeField] private float timeForTextDisappear;

        private Color _initialColor;
        private float _remainingTime;
        private float _holdStartTime;
        private bool _isHoldingKey;
        private void Awake()
        {
            playerCellsInShop.OnAbilityChanged += SwitchAbility;
            _initialColor = abilityImage.color;
            cooldownText.gameObject.SetActive(false);
            CurrentAbility = initialAbility;
        }
        
        private void OnDestroy()
        {
            playerCellsInShop.OnAbilityChanged -= SwitchAbility;
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyToUse))
            {
                if (!CurrentAbility.CanUse)
                {
                    return;
                }
                
                if (CurrentAbility.AbilityType == AbilityTypes.TapButton)
                {
                    //todo: refactor
                    CurrentAbility.UseAbility();
                    var cooldownSecs = (float)CurrentAbility.CooldownMilliseconds / 1000;
                    ShowCooldownStarted(cooldownSecs);
                    StartTimeTracking(cooldownSecs).Forget();
                }
                else
                {
                    //todo get here cts
                    ShowHoldButtonText(CancellationToken.None).Forget();
                }
            }

            if (Input.GetKey(keyToUse))
            {
                if (!CurrentAbility.CanUse)
                {
                    return;
                }
                
                if (CurrentAbility.AbilityType == AbilityTypes.HoldButton)
                {
                    UseAbilityOnKeyHold();
                }
            }
        }
        private void UseAbilityOnKeyHold()
        {
            if (Input.GetKeyDown(keyToUse))
            {
                _holdStartTime = Time.time;
                _isHoldingKey = true;
            }
        
            if (Input.GetKey(keyToUse) && _isHoldingKey)
            {
                if (Time.time - _holdStartTime >= timeToHoldKey)
                {
                    CurrentAbility.UseAbility();
                    var cooldownSecs = (float)CurrentAbility.CooldownMilliseconds / 1000;
                    ShowCooldownStarted(cooldownSecs);
                    StartTimeTracking(cooldownSecs).Forget();
                    
                    _isHoldingKey = false;
                }
            }
        
            if (Input.GetKeyUp(keyToUse))
            {
                _isHoldingKey = false;
            }
        }
        private async UniTask ShowHoldButtonText(CancellationToken token)
        {
            holdKeyText.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(timeForTextDisappear), cancellationToken: token);
            holdKeyText.gameObject.SetActive(false);
        }
        private async UniTask StartTimeTracking(float cooldown)
        {
            _remainingTime = cooldown;

            while (_remainingTime > 0)
            {
                cooldownText.text = Mathf.CeilToInt(_remainingTime).ToString();

                await UniTask.Yield(PlayerLoopTiming.Update);

                _remainingTime -= Time.deltaTime;
            }
            ShowCooldownEnded();
        }

        private void ShowCooldownStarted(float cooldown)
        {
            abilityImage.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, transparencyOnCooldown);
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = Mathf.CeilToInt(cooldown).ToString();
        }

        private void ShowCooldownEnded()
        {
            abilityImage.color = _initialColor;
            cooldownText.gameObject.SetActive(false);
        }
        private void SwitchAbility(int abilityCellIndex, Ability newAbility)
        {
            if (abilityCellIndex == cellIndex)
            {
                CurrentAbility = newAbility;
                abilityImage.sprite = CurrentAbility.AbilityImage;
                Debug.Log("Ability switched");
            }
        }
        
    }
}