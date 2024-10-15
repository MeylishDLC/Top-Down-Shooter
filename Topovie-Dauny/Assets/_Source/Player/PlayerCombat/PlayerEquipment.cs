using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using Player.PlayerAbilities;
using Player.PlayerControl;
using Player.PlayerControl.GunMovement;
using UI;
using UI.PlayerGUI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Weapons;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerEquipment: MonoBehaviour
    {
        public PlayerWeaponsSetter PlayerWeaponsSetter { get; private set; }
        
        [Header("Weapons")]
        [SerializeField] private SerializedDictionary<KeyCode, Gun> weaponsObjects;
        [SerializeField] private GunRotation gunRotation;
        [SerializeField] private Image gunUIImage;
        [SerializeField] private Color gunKeyColorEnabled;
        [SerializeField] private Color gunKeyColorDisabled;

        [Header("Abilities")] 
        [SerializeField] private UIPlayerEquipmentCell[] uiPlayerEquipmentCells;

        private PlayerMovement _playerMovement;
        private DialogueManager _dialogueManager;
        private UI.UIShop.Shop _shop;

        [Inject]
        public void Construct(PlayerMovement playerMovement, DialogueManager dialogueManager, 
            UI.UIShop.Shop shop)
        {
            _playerMovement = playerMovement;
            _dialogueManager = dialogueManager;
            _shop = shop;
            PlayerWeaponsSetter = new PlayerWeaponsSetter(weaponsObjects, gunRotation, gunUIImage, gunKeyColorEnabled, gunKeyColorDisabled);
        }
        private void Start()
        {
            SubscribeOnEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeOnEvents();
        }
        private void Update()
        {
            if (!_dialogueManager.DialogueIsPlaying && !_shop.ShopIsOpen)
            {
                PlayerWeaponsSetter.HandleShooting();
            }
        }
        private void EnableAbility(Ability ability)
        {
            ability.Construct(_playerMovement);
            ability.UseAbility();
        }
        private void SubscribeOnEvents()
        {
            foreach (var cell in uiPlayerEquipmentCells)
            {
                cell.OnUseAbility += EnableAbility;
            }
        }
        private void UnsubscribeOnEvents()
        {
            foreach (var cell in uiPlayerEquipmentCells)
            {
                cell.OnUseAbility -= EnableAbility;
            }
        }
    }
}