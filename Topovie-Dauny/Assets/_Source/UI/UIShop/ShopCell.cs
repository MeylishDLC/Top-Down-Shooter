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
        [SerializeField] private GameObject equipScreen;
        [SerializeField] private InfoPanel infoPanel;

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
            infoPanel.gameObject.SetActive(true);
            infoPanel.ChangeInfo(ability);
            
            equipImage.gameObject.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            infoPanel.gameObject.SetActive(false);
            equipImage.gameObject.SetActive(false);
        }
        private void EnterEquipAbilityMode()
        {
            equipScreen.gameObject.SetActive(true);
            OnEquipAbilityModeEntered?.Invoke(ability);
        }
      
    }
}