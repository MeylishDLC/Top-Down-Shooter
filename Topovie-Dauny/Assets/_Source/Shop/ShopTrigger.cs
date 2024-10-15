using System;
using Core;
using UI;
using UnityEngine;
using Zenject;

namespace Shop
{
    public class ShopTrigger: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private UI.UIShop.Shop _shop;

        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;

        private LevelSetter _levelSetter;
        
        [Inject]
        public void Construct(LevelSetter levelSetter)
        {
            _levelSetter = levelSetter;
        }
        private void Awake()
        {
            _levelSetter.OnStateChanged += EnableOnChangeState;
            visualQue.gameObject.SetActive(false);
            EnableOnChangeState(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _levelSetter.OnStateChanged -= EnableOnChangeState;
        }
        
        private void Update()
        {

            if (_playerInRange)
            {
                visualQue.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    _shop.OpenShop();
                }
            }
            else
            {
                visualQue.gameObject.SetActive(false);
            }
        }
        private void EnableOnChangeState(GameStates gameState)
        {
            enabled = gameState == GameStates.Chill;
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