using System;
using Player.PlayerAbilities;
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
        
        private Button equipButton;
        private void Awake()
        {
            equipButton = GetComponent<Button>();
            equipButton.onClick.AddListener(EnterEquipAbilityMode);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.ChangeInfo(ability);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            infoPanel.gameObject.SetActive(false);
        }
        private void EnterEquipAbilityMode()
        {
            equipScreen.gameObject.SetActive(true);
            OnEquipAbilityModeEntered?.Invoke(ability);
        }
      
    }
}