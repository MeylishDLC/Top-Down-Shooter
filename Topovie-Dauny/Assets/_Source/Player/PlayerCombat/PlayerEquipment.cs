using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UI;
using UI.PlayerGUI;
using UnityEngine;
using Weapons;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerEquipment: MonoBehaviour
    {
        public PlayerWeapons PlayerWeapons { get; private set; }
        
        [Header("Weapons")]
        [SerializeField] private SerializedDictionary<KeyCode, GameObject> weaponsObjects;

        [Header("Abilities")] 
        [SerializeField] private PlayerEquipmentCell[] equipmentCells;
        
        private DialogueManager _dialogueManager;
        private UI.UIShop.Shop _shop;

        [Inject]
        public void Construct(DialogueManager dialogueManager, UI.UIShop.Shop shop)
        {
            _dialogueManager = dialogueManager;
            _shop = shop;
            PlayerWeapons = new PlayerWeapons(weaponsObjects);
        }
        private void Update()
        {
            if (!_dialogueManager.DialogueIsPlaying && !_shop.ShopIsOpen)
            {
                PlayerWeapons.HandleShooting();
            }
        }
    }
}