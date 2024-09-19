using System;
using UnityEngine;

namespace Core
{
    public class PortalChargerTrigger: MonoBehaviour
    {
        public event Action<int> OnChargePortalPressed;

        [SerializeField] private int chargeIndex;
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private float holdChargePortalButtonDuration;
        
        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;
        private void Start()
        {
            visualQue.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_playerInRange)
            {
                visualQue.gameObject.SetActive(true);
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
            if (Input.GetKeyDown(KeyCode.F))
            {
                _holdStartTime = Time.time;
                _isHoldingButton = true;
            }
        
            if (Input.GetKey(KeyCode.F) && _isHoldingButton)
            {
                if (Time.time - _holdStartTime >= holdChargePortalButtonDuration)
                {
                    OnChargePortalPressed?.Invoke(chargeIndex);
                    Debug.Log("Portal Charge Started");
                    
                    visualQue.gameObject.SetActive(false);
                    _isHoldingButton = false;
                    
                    enabled = false;
                }
            }
        
            if (Input.GetKeyUp(KeyCode.F))
            {
                _isHoldingButton = false;
            }
        }
    }
}