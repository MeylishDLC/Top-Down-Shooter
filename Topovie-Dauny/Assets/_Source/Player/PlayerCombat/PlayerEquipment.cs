using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using Player.PlayerMovement.GunMovement;
using UI;
using UI.PlayerGUI;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerEquipment: MonoBehaviour
    {
        public PlayerWeapons PlayerWeapons { get; private set; }
        
        [Header("Weapons")]
        [SerializeField] private SerializedDictionary<KeyCode, Gun> weaponsObjects;
        [SerializeField] private GunRotation gunRotation;
        [SerializeField] private Image gunUIImage;

        [Header("Abilities")] 
        
        private DialogueManager _dialogueManager;
        private UI.UIShop.Shop _shop;

        [Inject]
        public void Construct(DialogueManager dialogueManager, UI.UIShop.Shop shop)
        {
            _dialogueManager = dialogueManager;
            _shop = shop;
            PlayerWeapons = new PlayerWeapons(weaponsObjects, gunRotation, gunUIImage);
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