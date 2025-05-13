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

        [SerializeField] private ChargerZone chargerZone;
        [SerializeField] private int chargeIndex;
        [SerializeField] private SpriteRenderer visualQue;
        [SerializeField] private float holdChargePortalButtonDuration;
        
        private bool _isHoldingButton;
        private float _holdStartTime;
        private bool _playerInRange;
        private InputListener _inputListener;
        private StatesChanger _statesChanger;
        private CancellationToken _ctOnDestroy;
        
        [Inject]
        public void Construct(StatesChanger statesChanger, InputListener inputListener)
        {
            _inputListener = inputListener;
            _statesChanger = statesChanger;
            
            SubscribeOnEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeOnEvents();
        }
        private void Start()
        {
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();
            visualQue.gameObject.SetActive(false);
            EnableOnChangeStatesChanger(GameStates.Chill);
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

            if (_statesChanger.CurrentGameState != GameStates.Chill)
            {
                return;
            }
            
            chargerZone.BeginCharge();
            StartHoldingChargeButtonAsync(_ctOnDestroy).Forget();
        }
        private async UniTask StartHoldingChargeButtonAsync(CancellationToken token)
        {
            _holdStartTime = Time.time;
            _isHoldingButton = true;

            while (_isHoldingButton)
            {
                if (_statesChanger.CurrentGameState != GameStates.Chill)
                {
                    break;
                }
                
                if (Time.time - _holdStartTime >= holdChargePortalButtonDuration)
                {
                    OnChargePortalPressed?.Invoke(chargeIndex);
                    UnsubscribeOnEvents();
                    
                    chargerZone.ForceFill();
                    
                    visualQue.gameObject.SetActive(false);
                    _isHoldingButton = false;
                    enabled = false;
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private void ReleaseChargeButton()
        {
            chargerZone.CancelCharge();
            _isHoldingButton = false;
        }
        private void EnableOnChangeStatesChanger(GameStates state)
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
        private void SubscribeOnEvents()
        {
            _statesChanger.OnStateChanged += EnableOnChangeStatesChanger;
            _inputListener.OnInteractPressed += StartHoldingChargeButton;
            _inputListener.OnInteractReleased += ReleaseChargeButton;
        }
        private void UnsubscribeOnEvents()
        {
            _statesChanger.OnStateChanged -= EnableOnChangeStatesChanger;
            _inputListener.OnInteractPressed -= StartHoldingChargeButton;
            _inputListener.OnInteractReleased -= ReleaseChargeButton;
        }
    }
}