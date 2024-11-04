using System;
using System.Threading;
using Core.InputSystem;
using Core.LevelSettings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace GameEnvironment
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
        private LevelChargesHandler _levelChargesHandler;
        private InputListener _inputListener;
        
        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler, InputListener inputListener)
        {
            _inputListener = inputListener;
            _levelChargesHandler = levelChargesHandler;
            _levelChargesHandler.OnStateChanged += EnableOnChangeState;
            _inputListener.OnInteractPressed += StartHoldingChargeButton;
            _inputListener.OnInteractReleased += ReleaseChargeButton;
        }
        private void OnDestroy()
        {
            _levelChargesHandler.OnStateChanged -= EnableOnChangeState;
            _inputListener.OnInteractPressed -= StartHoldingChargeButton;
            _inputListener.OnInteractReleased -= ReleaseChargeButton;
        }
        private void Start()
        {
            visualQue.gameObject.SetActive(false);
            EnableOnChangeState(GameStates.Chill);
        }
        private void Update()
        {
            if (_playerInRange)
            {
                visualQue.gameObject.SetActive(true);
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
        private void StartHoldingChargeButton()
        {
            if (!_playerInRange)
            {
                return;
            }
            
            StartHoldingChargeButtonAsync(CancellationToken.None).Forget();
        }
        private async UniTask StartHoldingChargeButtonAsync(CancellationToken token)
        {
            _holdStartTime = Time.time;
            _isHoldingButton = true;

            while (_isHoldingButton)
            {
                if (Time.time - _holdStartTime >= holdChargePortalButtonDuration)
                {
                    OnChargePortalPressed?.Invoke(chargeIndex);
                    _levelChargesHandler.OnStateChanged -= EnableOnChangeState;
                    Debug.Log("Portal Charge Started");
                    
                    visualQue.gameObject.SetActive(false);
                    _isHoldingButton = false;
                    
                    enabled = false;
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private void ReleaseChargeButton()
        {
            _isHoldingButton = false;
        }
        private void EnableOnChangeState(GameStates state)
        {
            if (state != GameStates.Fight)
            {
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }
        
    }
}