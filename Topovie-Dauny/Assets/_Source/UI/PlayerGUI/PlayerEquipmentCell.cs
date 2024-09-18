using System;
using System.Globalization;
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
        
        [Header("UI")]
        [SerializeField] private Image abilityImage;
        [Range(0f,1f)][SerializeField] private float transparencyOnCooldown;
        [SerializeField] private TMP_Text cooldownText;

        private Color _initialColor;
        private float _remainingTime;
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
                if (CurrentAbility.CanUse)
                {
                    CurrentAbility.UseAbility();
                    var cooldownSecs = (float)CurrentAbility.CooldownMilliseconds / 1000;
                    ShowCooldownStarted(cooldownSecs);
                    StartTimeTracking(cooldownSecs).Forget();
                }
            }
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