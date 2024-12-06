using System;
using System.Collections.Generic;
using System.Linq;
using Player.PlayerAbilities;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.UIShop
{
    public class PlayerCellsInShop: MonoBehaviour
    {
        public event Action<int, Ability> OnAbilityChanged;
        [field:SerializeField] public Button[] EquipmentCells { get; private set; }

        [SerializeField] private Ability[] abilitiesOnStart;
        [SerializeField] private ShopCell[] shopCells;
        [SerializeField] private GameObject equipScreen;

        private readonly Dictionary<Button, Ability> _equipmentCellAbilities = new();
        private readonly List<Image> _equipmentCellImages = new();
        private Ability _newAbilityToEquip;
        private AudioManager _audioManager;
        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        private void Awake()
        {
            InitializeImages();
            InitializeDictionary();
            
            ChangeButtonsInteractable(false);
            SubscribeOnEvents(true);
            SetEquipmentCellsOnStart();
        }
        private void OnDestroy()
        {
            SubscribeOnEvents(false);
        }
        public void OnEquipmentCellChoose(int cellIndex)
        {
            _audioManager.PlayOneShot(_audioManager.FMODEvents.ShopButtonSound);
            equipScreen.SetActive(false);
            ChangeButtonsInteractable(false);
            
            if (IsNewAbilitySetInOtherCells(cellIndex, out var indexOfOtherCell))
            {
                //move ability to opposite cell
                var abilityInTheCellChosen = _equipmentCellAbilities[EquipmentCells[cellIndex]];
                
                SetNewAbilityInCell(cellIndex, _newAbilityToEquip);
                SetNewAbilityInCell(indexOfOtherCell, abilityInTheCellChosen);
                OnAbilityChanged?.Invoke(indexOfOtherCell, abilityInTheCellChosen);
            }
            else
            {
                SetNewAbilityInCell(cellIndex, _newAbilityToEquip);
            }
            OnAbilityChanged?.Invoke(cellIndex, _newAbilityToEquip);
        }
        private void SetNewAbilityInCell(int cellIndex, Ability abilityToEquip)
        {
            //todo refactor
            _equipmentCellAbilities[EquipmentCells[cellIndex]] = abilityToEquip;
            var image = EquipmentCells[cellIndex].gameObject.transform.GetChild(0).GetComponent<Image>();
            image.sprite = abilityToEquip.AbilityImage;
        }
        private bool IsNewAbilitySetInOtherCells(int cellChosen, out int indexOfOtherCell)
        {
            indexOfOtherCell = -1;
            for (var i = 0; i < EquipmentCells.Length; i++)
            {
                if (i == cellChosen)
                {
                    continue;
                } 
                var image = EquipmentCells[i].gameObject.transform.GetChild(0).GetComponent<Image>();
                if (image.sprite == _newAbilityToEquip.AbilityImage)
                {
                    indexOfOtherCell = i;
                    return true;
                }
            }
            return false;
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
        private void InitializeImages()
        {
            foreach (var equipmentCell in EquipmentCells)
            {
                var image = equipmentCell.gameObject.transform.GetChild(0);
                _equipmentCellImages.Add(image.GetComponent<Image>());
            }
        }
        private void InitializeDictionary()
        {
            for (int i = 0; i < EquipmentCells.Length; i++)
            {
                _equipmentCellAbilities.Add(EquipmentCells[i], abilitiesOnStart[i]);
            }
        }
        private void EnterEquipAbilityMode(Ability abilityToEquip)
        {
            ChangeButtonsInteractable(true);
            _newAbilityToEquip = abilityToEquip;
        }
        private void SubscribeOnEvents(bool subscribe)
        {
            if (subscribe)
            {
                foreach (var shopCell in shopCells)
                {
                    shopCell.OnEquipAbilityModeEntered += EnterEquipAbilityMode;
                }
            }
            else
            {
                foreach (var shopCell in shopCells)
                {
                    shopCell.OnEquipAbilityModeEntered -= EnterEquipAbilityMode;
                }
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