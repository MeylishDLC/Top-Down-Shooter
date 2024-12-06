using System;
using Player.PlayerAbilities;
using SoundSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI.UIShop
{
    public class ShopCell: MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<Ability> OnEquipAbilityModeEntered; 
        [field:SerializeField] public Ability ability { get; private set; }
        [Header("UI")] 
        [SerializeField] private Image equipImage;
        [SerializeField] private Color equipColor;
        
        private AudioManager _audioManager;
        private Button _equipButton;
        private Shop _shop;
        [Inject]
        public void Construct(Shop shop, AudioManager audioManager)
        {
            _shop = shop;
            _audioManager = audioManager;
        }
        private void Awake()
        {
            _equipButton = GetComponent<Button>();
            _equipButton.onClick.AddListener(EnterEquipAbilityMode);

            equipImage.color = equipColor;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowInfo();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            HideInfo();
        }
        private void ShowInfo()
        {
            _shop.InfoPanel.gameObject.SetActive(true);
            _shop.InfoPanel.ChangeInfo(ability);
            
            equipImage.gameObject.SetActive(true);
        }
        private void HideInfo()
        {
            _shop.InfoPanel.gameObject.SetActive(false);
            equipImage.gameObject.SetActive(false);
        }
        private void EnterEquipAbilityMode()
        {
            _audioManager.PlayOneShot(_audioManager.FMODEvents.ShopButtonSound);
            _shop.EquipScreen.gameObject.SetActive(true);
            OnEquipAbilityModeEntered?.Invoke(ability);
        }
      
    }
}