﻿using Core.InputSystem;
using Core.LevelSettings;
using DialogueSystem;
using UI.UIShop;
using UnityEngine;
using Zenject;

namespace GameEnvironment
{
    public class ShopTrigger: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualQue;

        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;

        private LevelChargesHandler _levelChargesHandler;
        private InputListener _inputListener;
        private Shop _shop;
        private DialogueManager _dialogueManager;
        
        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler, InputListener inputListener, Shop shop, 
            DialogueManager dialogueManager)
        {
            _dialogueManager = dialogueManager;
            _shop = shop;
            _levelChargesHandler = levelChargesHandler;
            _inputListener = inputListener;
        }
        private void Awake()
        {
            _levelChargesHandler.OnStateChanged += EnableOnChangeState;
            _inputListener.OnInteractPressed += ShowShop;
            
            visualQue.gameObject.SetActive(false);
            EnableOnChangeState(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _levelChargesHandler.OnStateChanged -= EnableOnChangeState;
            _inputListener.OnInteractPressed -= ShowShop;
        }
        private void ShowShop()
        {
            if (_dialogueManager.DialogueIsPlaying)
            {
                return;
            }
            
            if (_playerInRange)
            {
                _shop.OpenShop();
            }
        }
        private void Update()
        {
            visualQue.gameObject.SetActive(_playerInRange);
        }
        private void EnableOnChangeState(GameStates gameState)
        {
            if (gameState != GameStates.Fight)
            {
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _playerInRange = false;
            }
        }
    }
}