using Core.InputSystem;
using DialogueSystem;
using Player.PlayerAbilities;
using Player.PlayerControl;
using UI.PlayerGUI;
using UI.UIShop;
using UnityEngine;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerEquipment: MonoBehaviour
    {
        [Header("Abilities")] 
        [SerializeField] private UIPlayerEquipmentCell[] uiPlayerEquipmentCells;
        
        private PlayerMovement _playerMovement;
        private DialogueManager _dialogueManager;
        private Shop _shop;
        private InputListener _inputListener;
        private WeaponsSetter _weaponsSetter;

        [Inject]
        public void Construct(PlayerMovement playerMovement, DialogueManager dialogueManager, 
            Shop shop, InputListener inputListener, WeaponsSetter weaponsSetter)
        {
            _playerMovement = playerMovement;
            _weaponsSetter = weaponsSetter;
            _inputListener = inputListener;
            _dialogueManager = dialogueManager;
            _shop = shop;
            
            _weaponsSetter.SubscribeOnInputEvents(_inputListener);
        }
        private void Start()
        {
            SubscribeOnEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeOnEvents();
            _weaponsSetter.UnsubscribeOnInputEvents(_inputListener);
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