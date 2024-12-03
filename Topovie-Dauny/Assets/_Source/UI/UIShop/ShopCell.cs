using System;
using Player.PlayerAbilities;
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
        
        private Button equipButton;
        private Shop _shop;

        private void Awake()
        {
            equipButton = GetComponent<Button>();
            equipButton.onClick.AddListener(EnterEquipAbilityMode);

            equipImage.color = equipColor;
        }

        [Inject]
        public void Construct(Shop shop)
        {
            _shop = shop;
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
            _shop.EquipScreen.gameObject.SetActive(true);
            OnEquipAbilityModeEntered?.Invoke(ability);
        }
      
    }
}