using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UI;
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
        
        private DialogueManager _dialogueManager;
        private UIShopDisplay _uiShopDisplay;

        [Inject]
        public void Construct(DialogueManager dialogueManager, UIShopDisplay uiShopDisplay)
        {
            _dialogueManager = dialogueManager;
            _uiShopDisplay = uiShopDisplay;
            PlayerWeapons = new PlayerWeapons(weaponsObjects);
        }
        private void Update()
        {
            if (!_dialogueManager.DialogueIsPlaying && !_uiShopDisplay.ShopIsOpen)
            {
                PlayerWeapons.HandleShooting();
            }
        }
    }
}