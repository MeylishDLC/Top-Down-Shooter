using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Shop
{
    public class ShopTrigger: MonoBehaviour
    {
        public event Action OnChargePortalPressed;
        
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private GameObject shop;
        [SerializeField] private GameObject playerGUI;
        [SerializeField] private float holdChargePortalButtonDuration;
        
        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;
        
        //todo finish
        private void Start()
        {
            visualQue.gameObject.SetActive(false);
            shop.SetActive(false);
            playerGUI.SetActive(true);
        }
        private void Update()
        {
            if (_playerInRange)
            {
                visualQue.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    shop.SetActive(true);
                    playerGUI.SetActive(false);
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
            _playerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _playerInRange = false;
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