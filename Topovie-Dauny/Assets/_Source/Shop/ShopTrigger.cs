using System;
using UI;
using UnityEngine;
using Zenject;

namespace Shop
{
    public class ShopTrigger: MonoBehaviour
    {
        public event Action OnChargePortalPressed;
        
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private float holdChargePortalButtonDuration;
        
        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;
        private UIShopDisplay _uiShopDisplay;

        [Inject]
        public void Construct(UIShopDisplay uiShopDisplay)
        {
            _uiShopDisplay = uiShopDisplay;
        }
        private void Start()
        {
            visualQue.gameObject.SetActive(false);
            _uiShopDisplay.CloseShop();
        }
        private void Update()
        {
            if (_playerInRange)
            {
                visualQue.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    _uiShopDisplay.OpenShop();
                }
                HandleChargePortalButtonHold();
            }
            else
            {
                visualQue.gameObject.SetActive(false);
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

        private void HandleChargePortalButtonHold()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                _holdStartTime = Time.time;
                _isHoldingButton = true;
            }
        
            if (Input.GetKey(KeyCode.G) && _isHoldingButton)
            {
                if (Time.time - _holdStartTime >= holdChargePortalButtonDuration)
                {
                    OnChargePortalPressed?.Invoke();
                    Debug.Log("Portal Charge Started");
                    
                    gameObject.SetActive(false);
                    _isHoldingButton = false;
                }
            }
        
            if (Input.GetKeyUp(KeyCode.G))
            {
                _isHoldingButton = false;
            }
        }
    }
}