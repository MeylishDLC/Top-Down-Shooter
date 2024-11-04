using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.InputSystem
{ 
    public class InputListener : MonoBehaviour
    {
        public event Action<int> OnSwitchWeaponPressed; 
        public event Action OnFirePressed;
        public event Action OnFireReleased;
        public event Action OnRollPressed;
        
        private InputActions _inputActions;

        private InputAction _moveAction;
        private InputAction _fireAction;
        private InputAction _rollAction;
        
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
        }
        private void OnDisable()
        {
            DisableMovementActions();
            DisableWeaponSwitchActions();
        }
        private void OnDestroy()
        {
            ExposeMoveActions();
            ExposeWeaponSwitchActions();
        }
        public Vector2 GetMovementValue()
        {
            return _moveAction.ReadValue<Vector2>();
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

        private void EnableWeaponSwitchActions()
        {
            _inputActions.Player.WeaponSwitch1.Enable();
            _inputActions.Player.WeaponSwitch2.Enable();
            _inputActions.Player.WeaponSwitch3.Enable();
            _inputActions.Player.WeaponSwitch4.Enable();
        }
        #region ACTIONS_SETUP
        private void SetupMoveAction()
        {
            _moveAction = _inputActions.Player.Move;
            _moveAction.Enable();
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
        private void SetupSwitchWeaponButtons()
        {
            _inputActions.Player.WeaponSwitch1.started += OnSwitchWeapon1Pressed;
            _inputActions.Player.WeaponSwitch2.started += OnSwitchWeapon2Pressed;
            _inputActions.Player.WeaponSwitch3.started += OnSwitchWeapon3Pressed;
            _inputActions.Player.WeaponSwitch4.started += OnSwitchWeapon4Pressed;
            
            _inputActions.Player.WeaponSwitch1.Enable();
            _inputActions.Player.WeaponSwitch2.Enable();
            _inputActions.Player.WeaponSwitch3.Enable();
            _inputActions.Player.WeaponSwitch4.Enable();
        }
        #endregion

        #region ACTIONS_EXPOSE
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
        #endregion

        #region ACTIONS_DISABLE

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
        #endregion
    }
}