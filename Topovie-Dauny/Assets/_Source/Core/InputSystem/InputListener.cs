using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.InputSystem
{ 
    public class InputListener : MonoBehaviour
    {
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
        }
        private void OnDisable()
        {
            _moveAction.Disable();
            _fireAction.Disable();
        }
        private void OnDestroy()
        {
            _fireAction.started -= OnFireButtonPressed;
            _fireAction.canceled -= OnFireButtonReleased;
            _rollAction.started -= OnRollButtonPressed;
        }
        public Vector2 GetMovementValue()
        {
            return _moveAction.ReadValue<Vector2>();
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
        #endregion
    }
}