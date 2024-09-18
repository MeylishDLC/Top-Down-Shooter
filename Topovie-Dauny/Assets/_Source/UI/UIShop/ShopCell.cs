using System;
using Player.PlayerAbilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UIShop
{
    public class ShopCell: MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<Ability> OnEquipAbilityModeEntered; 
        
        [SerializeField] private Ability ability;
        [SerializeField] private Shop shop;

        [Header("UI")] 
        [SerializeField] private Image equipImage;
        [SerializeField] private Color equipColor;
        
        private Button equipButton;
        private void Awake()
        {
            equipButton = GetComponent<Button>();
            equipButton.onClick.AddListener(EnterEquipAbilityMode);

            equipImage.color = equipColor;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            shop.InfoPanel.gameObject.SetActive(true);
            shop.InfoPanel.ChangeInfo(ability);
            
            equipImage.gameObject.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            shop.InfoPanel.gameObject.SetActive(false);
            equipImage.gameObject.SetActive(false);
        }
        private void EnterEquipAbilityMode()
        {
            shop.EquipScreen.gameObject.SetActive(true);
            OnEquipAbilityModeEntered?.Invoke(ability);
        }
      
    }
}