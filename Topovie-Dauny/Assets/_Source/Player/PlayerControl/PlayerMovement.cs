using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using GameEnvironment.ShopLogic.UIShop;
using Player.PlayerCombat;
using Player.PlayerControl.GunMovement;
using UnityEngine;
using Zenject;

namespace Player.PlayerControl
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public event Action OnPlayerDeathEnd;
        public float MovementSpeed { get; private set; }

        [SerializeField] private Animator[] sides;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private GunRotation gunRotation;

        private float _dodgeSpeed;
        private float _dodgeTime;
        private bool _dodgeRoll;
        
        private Vector2 _direction;
        private float _horizontal;
        private float _vertical;

        private bool _isDying;
        private float _playerDeathDuration;
        
        private CancellationToken _ctOnDestroy;
        private PlayerView _playerView;

        private Rigidbody2D _rb;
        private Shop _shop;
        private InputListener _inputListener;
        private DialogueManager _dialogueManager;
        
        [Inject]
        public void Construct(InputListener inputListener, DialogueManager dialogueManager, Shop shop, PlayerConfig playerConfig)
        {
            MovementSpeed = playerConfig.MovementSpeed;
            _dodgeSpeed = playerConfig.DodgeSpeed;
            _dodgeTime = playerConfig.DodgeTime;
            _playerDeathDuration = playerConfig.PlayerDeathAnimationDuration;
            
            _inputListener = inputListener;
            _dialogueManager = dialogueManager;
            _shop = shop;
        }
        private void Awake()
        {
            //TODO MOVE TO INSTALLER
            _playerView = new PlayerView(sides);
            
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();
            
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _inputListener.OnRollPressed += HandleRolling;
            playerHealth.OnDeath += Die;
            
            _isDying = false;
        }
        private void OnDestroy()
        {
            _inputListener.OnRollPressed -= HandleRolling;
            playerHealth.OnDeath -= Die;
        }
        private void Update()
        {
            if (_dialogueManager.DialogueIsPlaying || _shop.IsShopOpen() || _isDying)
            {
                ForceDisableMovement();
            }
            else
            {
                ForceEnableMovement();
            }
            
            if (!_dialogueManager.DialogueIsPlaying && !playerHealth.IsKnockedBack &&
                !_shop.IsShopOpen() && !_isDying)
            {
                if (_dodgeRoll)
                {
                    _rb.AddForce(_direction * _dodgeSpeed);
                }
                HandleMovement();
            }
        }
        private void FixedUpdate()
        {
            if (!_dialogueManager.DialogueIsPlaying && !playerHealth.IsKnockedBack &&
                !_shop.IsShopOpen())
            {
                _rb.velocity = new Vector2(_horizontal, _vertical).normalized * MovementSpeed;
            }
        }
        public void ChangeSpeed(float newSpeed)
        {
            MovementSpeed = newSpeed;
        }
        private void HandleMovement()
        {
            _horizontal = _inputListener.GetMovementValue().x;
            _vertical = _inputListener.GetMovementValue().y;

            if (_horizontal > 0 || _horizontal < 0 || _vertical < 0 || _vertical > 0)
            {
                _playerView.HandleWalkingAnimation(true);
            }
            else
            {
                _playerView.HandleWalkingAnimation(false);
            }

            _direction = new Vector2(_horizontal, _vertical);
        }
        private void HandleRolling()
        {
            if (!CanRoll())
            {
                return;
            }
            
            _playerView.PlayRollAnimation();
            RollAsync(_ctOnDestroy).Forget();
        }

        private bool CanRoll()
        {
            if (CheckRolling.IsRolling)
            {
                return false;
            }
            if (_rb.bodyType == RigidbodyType2D.Static)
            {
                return false;
            }
            if (_horizontal == 0 && _vertical == 0)
            {
                return false;
            }
            return true;
        }
        private async UniTask RollAsync(CancellationToken token)
        {
            _dodgeRoll = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_dodgeTime), cancellationToken: token);
            _dodgeRoll = false;
        }
        private void ForceDisableMovement()
        {
            _rb.bodyType = RigidbodyType2D.Static;
        }
        private void ForceEnableMovement()
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
        private void Die()
        {
            DieAsync(_ctOnDestroy).Forget();
        }
        private async UniTask DieAsync(CancellationToken token)
        {
            playerHealth.OnDeath -= Die;
            
            _isDying = true;
            ForceDisableMovement();
            _inputListener.SetInput(false, true);
            
            //TODO DISABLE GUN MOVEMENT
            
            _playerView.PlayDeathAnimation();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_playerDeathDuration),
                    cancellationToken: _ctOnDestroy);
            }
            catch (OperationCanceledException)
            {
                //
            }
            finally
            {
                OnPlayerDeathEnd?.Invoke();
            }
        }
    }
}