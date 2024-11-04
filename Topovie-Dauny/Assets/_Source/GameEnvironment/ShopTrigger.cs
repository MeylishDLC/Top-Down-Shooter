using Core.InputSystem;
using Core.LevelSettings;
using UI.UIShop;
using UnityEngine;
using Zenject;

namespace GameEnvironment
{
    public class ShopTrigger: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private Shop shop;

        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;

        private LevelChargesHandler _levelChargesHandler;
        private InputListener _inputListener;
        
        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler, InputListener inputListener)
        {
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
            if (_playerInRange)
            {
                shop.OpenShop();
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