using Core;
using UI.UIShop;
using UnityEngine;
using Zenject;

namespace WorldShop
{
    public class ShopTrigger: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private Shop _shop;

        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;

        private LevelChargesHandler _levelChargesHandler;
        
        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler)
        {
            _levelChargesHandler = levelChargesHandler;
        }
        private void Awake()
        {
            _levelChargesHandler.OnStateChanged += EnableOnChangeState;
            visualQue.gameObject.SetActive(false);
            EnableOnChangeState(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _levelChargesHandler.OnStateChanged -= EnableOnChangeState;
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