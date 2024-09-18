using System;
using System.Collections.Generic;
using System.Linq;
using Player.PlayerAbilities;
using UI.PlayerGUI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.UIShop
{
    public class PlayerCellsInShop: MonoBehaviour
    {
        public event Action<int, Ability> OnAbilityChanged;
        
        [SerializeField] private Ability[] abilitiesOnStart;
        [field:SerializeField] public Button[] EquipmentCells;
        [SerializeField] private ShopCell[] shopCells;
        [SerializeField] private GameObject equipScreen;
        
        private List<Image> _equipmentCellImages = new();
        private Ability _newAbilityToEquip;
        private void Awake()
        {
            foreach (var equipmentCell in EquipmentCells)
            {
                var image = equipmentCell.gameObject.transform.GetChild(0);
                _equipmentCellImages.Add(image.GetComponent<Image>());
            }
            SetEquipmentCellsOnStart();
            ChangeButtonsInteractable(false);
            SubscribeOnEvents();
        }
        public void OnEquipmentCellChoose(int cellIndex)
        {
            Debug.Log($"Cell chosen: {cellIndex}");
            equipScreen.SetActive(false);
            ChangeButtonsInteractable(false);
            var image = EquipmentCells[cellIndex].gameObject.transform.GetChild(0).GetComponent<Image>();
            image.sprite = _newAbilityToEquip.AbilityImage;
            OnAbilityChanged?.Invoke(cellIndex, _newAbilityToEquip);
        }

        private void SetEquipmentCellsOnStart()
        {
            //set images
            for (int i = 0; i < abilitiesOnStart.Length; i++)
            {
                _equipmentCellImages.ElementAt(i).sprite = abilitiesOnStart[i].AbilityImage;
            }
            
            //set abilities in player GUI
            for (int i = 0; i < abilitiesOnStart.Length; i++)
            {
                OnAbilityChanged?.Invoke(i, abilitiesOnStart[i]);
            }
        }
        private void EnterEquipAbilityMode(Ability abilityToEquip)
        {
            Debug.Log("Entered equip ability mode");
            ChangeButtonsInteractable(true);
            _newAbilityToEquip = abilityToEquip;
        }
        private void SubscribeOnEvents()
        {
            foreach (var shopCell in shopCells)
            {
                shopCell.OnEquipAbilityModeEntered += EnterEquipAbilityMode;
            }
        }
        
        private void ChangeButtonsInteractable(bool interactable)
        {
            foreach (var cell in EquipmentCells)
            {
                cell.interactable = interactable;
            }
        }
    }
}