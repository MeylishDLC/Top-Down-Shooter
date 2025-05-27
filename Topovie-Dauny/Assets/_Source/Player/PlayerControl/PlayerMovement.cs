using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using GameEnvironment.ShopLogic.UIShop;
using Player.PlayerCombat;
using UnityEngine;
using Zenject;

namespace Player.PlayerControl
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public event Action OnPlayerDeathEnd;
        
        private static readonly int isWalking = Animator.StringToHash("isWalking");
        private static readonly int isRolling = Animator.StringToHash("isRolling");
        private static readonly int isDead = Animator.StringToHash("isDead");
        public float MovementSpeed { get; private set; }

        [SerializeField] private Animator[] sides;
        [SerializeField] private PlayerHealth playerHealth;

        private PlayerConfig _playerConfig;
        
        private Vector2 _direction;
        private bool _dodgeRoll;
        private bool _canRoll = true;
        private float _horizontal;
        private float _vertical;

        private Rigidbody2D _rb;
        private Shop _shop;
        private InputListener _inputListener;
        private DialogueManager _dialogueManager;

        private CancellationToken _ctOnDestroy;
        
        [Inject]
        public void Construct(InputListener inputListener, DialogueManager dialogueManager, Shop shop, PlayerConfig playerConfig)
        {
            _playerConfig = playerConfig;
            _inputListener = inputListener;
            _dialogueManager = dialogueManager;
            _shop = shop;
        }
        private void Awake()
        {
            MovementSpeed = _playerConfig.MovementSpeed;
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();
            
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _inputListener.OnRollPressed += HandleRolling;
            playerHealth.OnDeath += PlayDeathAnimation;
        }
        private void OnDestroy()
        {
            _inputListener.OnRollPressed -= HandleRolling;
            playerHealth.OnDeath -= PlayDeathAnimation;
        }
        private void Update()
        {
            if (_dialogueManager.DialogueIsPlaying || _shop.IsShopOpen())
            {
                DisableMovement();
            }
            else
            {
                EnableMovement();
            }
            
            if (!_dialogueManager.DialogueIsPlaying && !playerHealth.IsKnockedBack &&
                !_shop.IsShopOpen())
            {
                if (_dodgeRoll)
                {
                    _rb.AddForce(_direction * _playerConfig.DodgeSpeed);
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
                _canRoll = true;
                foreach (var side in sides)
                {
                    if (side.gameObject.activeSelf)
                    {
                        side.SetBool(isWalking, true);
                    }
                }
            }
            else
            {
                _canRoll = false;
                foreach (var side in sides)
                {
                    if (side.gameObject.activeSelf)
                    {
                        side.SetBool(isWalking, false);
                    }
                }
            }

            _direction = new Vector2(_horizontal, _vertical);
        }
        private void HandleRolling()
        {
            if (!_canRoll)
            {
                return;
            }
            if (!CheckRolling.IsRolling)
            {
                foreach (var side in sides)
                {
                    if (side.gameObject.activeSelf)
                    {
                        side.SetTrigger(isRolling);
                    }

                    RollAsync(_ctOnDestroy).Forget();
                }
            }
        }
        private async UniTask RollAsync(CancellationToken token)
        {
            _dodgeRoll = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_playerConfig.DodgeTime), cancellationToken: token);
            _dodgeRoll = false;
        }
        private void DisableMovement()
        {
            _rb.bodyType = RigidbodyType2D.Static;
            _canRoll = false;
        }
        private void EnableMovement()
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _canRoll = true;
        }
        private void PlayDeathAnimation()
        {
            DisableMovement();
            playerHealth.OnDeath -= PlayDeathAnimation;
            foreach (var side in sides)
            {
                if (side.gameObject.activeSelf)
                {
                    side.SetTrigger(isDead);
                }
                UniTask.Delay(TimeSpan.FromSeconds(_playerConfig.PlayerDeathAnimationDuration), 
                    cancellationToken: _ctOnDestroy).ContinueWith(() => OnPlayerDeathEnd?.Invoke()).Forget();
            }
        }
    }
}