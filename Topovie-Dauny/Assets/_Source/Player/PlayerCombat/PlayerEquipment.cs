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
        private InputListener _inputListener;
        private WeaponsSetter _weaponsSetter;
        private ProjectContext _projectContext;

        [Inject]
        public void Construct(PlayerMovement playerMovement, InputListener inputListener, WeaponsSetter weaponsSetter,
            ProjectContext projectContext)
        {
            _projectContext = projectContext;
            _playerMovement = playerMovement;
            _weaponsSetter = weaponsSetter;
            _inputListener = inputListener;
            
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
            ability.Construct(_playerMovement, _projectContext);
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