using DialogueSystem;
using Player.PlayerAbilities;
using Player.PlayerControl;
using Player.PlayerControl.GunMovement;
using UI.PlayerGUI;
using UI.UIShop;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerEquipment: MonoBehaviour
    {
        [Header("Weapons")]
        [SerializeField] private SerializedDictionary<KeyCode, Gun> weaponsObjects;
        [SerializeField] private GunRotation gunRotation;
        [SerializeField] private Image gunUIImage;
        [SerializeField] private Color gunKeyColorEnabled;
        [SerializeField] private Color gunKeyColorDisabled;

        [Header("Abilities")] 
        [SerializeField] private UIPlayerEquipmentCell[] uiPlayerEquipmentCells;
        
        private WeaponsSetter _weaponsSetter;
        private PlayerMovement _playerMovement;
        private DialogueManager _dialogueManager;
        private Shop _shop;

        [Inject]
        public void Construct(PlayerMovement playerMovement, DialogueManager dialogueManager, 
            Shop shop, WeaponsSetter weaponsSetter)
        {
            _playerMovement = playerMovement;
            _dialogueManager = dialogueManager;
            _shop = shop;
            _weaponsSetter = weaponsSetter;
            _weaponsSetter.Initialize(weaponsObjects, gunRotation, gunUIImage, gunKeyColorEnabled, gunKeyColorDisabled);
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
            if (!_dialogueManager.DialogueIsPlaying && !_shop.IsShopOpen())
            {
                _weaponsSetter.HandleShooting();
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