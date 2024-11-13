using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.InputSystem
{ 
    public class InputListener : MonoBehaviour
    {
        [SerializeField] private int weaponCountForLevel;
        public event Action<int> OnSwitchWeaponPressed; 
        public event Action<int> OnUseAbilityPressed;
        public event Action<int> OnUseAbilityReleased; 
        public event Action OnFirePressed;
        public event Action OnFireReleased;
        public event Action OnRollPressed;
        public event Action OnInteractPressed;
        public event Action OnInteractReleased;
        public event Action OnPausePressed;
        
        private InputActions _inputActions;

        private InputAction _moveAction;
        private InputAction _fireAction;
        private InputAction _rollAction;
        private InputAction _interactAction;
        private InputAction _pauseAction;
        
        private void Awake()
        {
            _inputActions = new InputActions();
        }
        private void OnEnable()
        {
            SetupMoveAction();
            SetupFireAction();
            SetupRollAction();
            SetupSwitchWeaponButtons();
            SetupAbilityActions();
            SetupInteractAction();
            SetupPauseAction();
        }
        private void OnDisable()
        {
            DisablePauseAction();
            DisableMovementActions();
            DisableWeaponSwitchActions();
            DisableUseAbilityActions();
            DisableInteractAction();
        }
        private void OnDestroy()
        {
            ExposePauseAction();
            ExposeMoveActions();
            ExposeWeaponSwitchActions();
            ExposeUseAbilityActions();
            ExposeInteractAction();
        }
        public Vector2 GetMovementValue()
        {
            return _moveAction.ReadValue<Vector2>();
        }
        public void SetInput(bool enable, bool enableInteractOnly = false)
        {
            if (enableInteractOnly)
            {
                if (enable)
                {
                    EnableUseAbilityActions();
                    EnableMovementActions();
                    EnableWeaponSwitchActions();
                    EnableUseAbilityActions();
                }
                else
                {
                    DisableUseAbilityActions();
                    DisableMovementActions();
                    DisableWeaponSwitchActions();
                    DisableUseAbilityActions();
                }
            }
            else
            {
                if (enable)
                {
                    _inputActions.Enable();
                }
                else
                {
                    _inputActions.Disable();
                }
            }
        }
        public void SetFiringAbility(bool enable)
        {
            if (enable)
            {
                _fireAction.Enable();
                EnableWeaponSwitchActions();
            }
            else
            {
                _fireAction.Disable();
                DisableWeaponSwitchActions();
            }
        }
        public void SetUseAbility(bool enable)
        {
            if (enable)
            {
                EnableUseAbilityActions();
            }
            else
            {
                DisableUseAbilityActions();
            }
        }
        private void OnPauseButtonPressed(InputAction.CallbackContext context)
        {
            OnPausePressed?.Invoke();
        }
        private void OnInteractButtonPressed(InputAction.CallbackContext context)
        {
            OnInteractPressed?.Invoke();
        }
        private void OnInteractButtonReleased(InputAction.CallbackContext context)
        {
            OnInteractReleased?.Invoke();
        }
        private void OnFireButtonPressed(InputAction.CallbackContext context)
        {
            OnFirePressed?.Invoke();
        }
        private void OnFireButtonReleased(InputAction.CallbackContext context)
        {
            OnFireReleased?.Invoke();
        }
        private void OnRollButtonPressed(InputAction.CallbackContext context)
        {
            OnRollPressed?.Invoke();
        }
        private void OnSwitchWeapon1Pressed(InputAction.CallbackContext context)
        {
            OnSwitchWeaponPressed?.Invoke(1);
        }
        private void OnSwitchWeapon2Pressed(InputAction.CallbackContext context)
        {
            OnSwitchWeaponPressed?.Invoke(2);
        }
        private void OnSwitchWeapon3Pressed(InputAction.CallbackContext context)
        {
            OnSwitchWeaponPressed?.Invoke(3);
        }
        private void OnSwitchWeapon4Pressed(InputAction.CallbackContext context)
        {
            OnSwitchWeaponPressed?.Invoke(4);
        }
        private void OnUseAbility1Pressed(InputAction.CallbackContext context)
        {
            OnUseAbilityPressed?.Invoke(1);
        }
        private void OnUseAbility1Released(InputAction.CallbackContext context)
        {
            OnUseAbilityReleased?.Invoke(1);
        }
        private void OnUseAbility2Pressed(InputAction.CallbackContext context)
        {
            OnUseAbilityPressed?.Invoke(2);
        }
        private void OnUseAbility2Released(InputAction.CallbackContext context)
        {
            OnUseAbilityReleased?.Invoke(2);
        }
        
        #region ACTIONS_SETUP
        private void SetupPauseAction()
        {
            _pauseAction = _inputActions.Player.Pause;
            _pauseAction.started += OnPauseButtonPressed;
            _pauseAction.Enable();
        }
        private void SetupMoveAction()
        {
            _moveAction = _inputActions.Player.Move;
            _moveAction.Enable();
        }
        private void SetupInteractAction()
        {
            _interactAction = _inputActions.Player.Interact;
            _interactAction.started += OnInteractButtonPressed;
            _interactAction.canceled += OnInteractButtonReleased;
            _interactAction.Enable();
        }
        private void SetupFireAction()
        {
            _fireAction = _inputActions.Player.Fire;
            _fireAction.started += OnFireButtonPressed;
            _fireAction.canceled += OnFireButtonReleased;
            _fireAction.Enable();
        }
        private void SetupRollAction()
        {
            _rollAction = _inputActions.Player.Roll;
            _rollAction.started += OnRollButtonPressed;
            _rollAction.Enable();
        }
        private void SetupAbilityActions()
        {
            _inputActions.Player.UseAbility1.started += OnUseAbility1Pressed;
            _inputActions.Player.UseAbility2.started += OnUseAbility2Pressed;
            _inputActions.Player.UseAbility1.canceled += OnUseAbility1Released;
            _inputActions.Player.UseAbility2.canceled += OnUseAbility2Released;
            EnableUseAbilityActions();
        }
        private void SetupSwitchWeaponButtons()
        {
            switch (weaponCountForLevel)
            {
                case 1:
                    _inputActions.Player.WeaponSwitch1.started += OnSwitchWeapon1Pressed;
                    break;
                case 2:
                    _inputActions.Player.WeaponSwitch1.started += OnSwitchWeapon1Pressed;
                    _inputActions.Player.WeaponSwitch2.started += OnSwitchWeapon2Pressed;
                    break;
                case 3:
                    _inputActions.Player.WeaponSwitch1.started += OnSwitchWeapon1Pressed;
                    _inputActions.Player.WeaponSwitch2.started += OnSwitchWeapon2Pressed;
                    _inputActions.Player.WeaponSwitch3.started += OnSwitchWeapon3Pressed;
                    break;
                case 4:
                    _inputActions.Player.WeaponSwitch1.started += OnSwitchWeapon1Pressed;
                    _inputActions.Player.WeaponSwitch2.started += OnSwitchWeapon2Pressed;
                    _inputActions.Player.WeaponSwitch3.started += OnSwitchWeapon3Pressed;
                    _inputActions.Player.WeaponSwitch4.started += OnSwitchWeapon4Pressed;
                    break;
                default:
                    throw new Exception($"Amount of weapons is not supported : {weaponCountForLevel}");
            }
            EnableWeaponSwitchActions();
        }
        #endregion

        #region ACTIONS_EXPOSE
        private void ExposePauseAction()
        {
            _pauseAction.started -= OnPauseButtonPressed;
        }
        private void ExposeMoveActions()
        {
            _fireAction.started -= OnFireButtonPressed;
            _fireAction.canceled -= OnFireButtonReleased;
            _rollAction.started -= OnRollButtonPressed;
        }
        private void ExposeWeaponSwitchActions()
        {
            _inputActions.Player.WeaponSwitch1.started -= OnSwitchWeapon1Pressed;
            _inputActions.Player.WeaponSwitch2.started -= OnSwitchWeapon2Pressed;
            _inputActions.Player.WeaponSwitch3.started -= OnSwitchWeapon3Pressed;
            _inputActions.Player.WeaponSwitch4.started -= OnSwitchWeapon4Pressed;
        }
        private void ExposeUseAbilityActions()
        {
            _inputActions.Player.UseAbility1.started -= OnUseAbility1Pressed;
            _inputActions.Player.UseAbility2.started -= OnUseAbility2Pressed;
            _inputActions.Player.UseAbility1.canceled -= OnUseAbility1Released;
            _inputActions.Player.UseAbility2.canceled -= OnUseAbility2Released;
        }
        private void ExposeInteractAction()
        {
            _interactAction.started -= OnInteractButtonPressed;
            _interactAction.canceled -= OnInteractButtonReleased;
        }
        #endregion

        #region ACTIONS_DISABLE
        private void DisablePauseAction()
        {
            _pauseAction.Disable();
        }
        private void DisableMovementActions()
        {
            _moveAction.Disable();
            _fireAction.Disable();
            _rollAction.Disable();
        }
        private void DisableWeaponSwitchActions()
        {
            _inputActions.Player.WeaponSwitch1.Disable();
            _inputActions.Player.WeaponSwitch2.Disable();
            _inputActions.Player.WeaponSwitch3.Disable();
            _inputActions.Player.WeaponSwitch4.Disable();
        }
        private void DisableUseAbilityActions()
        {
            _inputActions.Player.UseAbility1.Disable();
            _inputActions.Player.UseAbility2.Disable();
        }
        private void DisableInteractAction()
        {
            _interactAction.Disable();
        }
        #endregion

        #region ACTIONS_ENABLE

        private void EnableMovementActions()
        {
            _moveAction.Enable();
            _fireAction.Enable();
            _rollAction.Enable();
        }

        private void EnablePauseAction()
        {
            _pauseAction.Enable();
        }
        private void EnableWeaponSwitchActions()
        {
            _inputActions.Player.WeaponSwitch1.Enable();
            _inputActions.Player.WeaponSwitch2.Enable();
            _inputActions.Player.WeaponSwitch3.Enable();
            _inputActions.Player.WeaponSwitch4.Enable();
        }
        private void EnableUseAbilityActions()
        {
            _inputActions.Player.UseAbility1.Enable();
            _inputActions.Player.UseAbility2.Enable();
        }
        #endregion
    }
}