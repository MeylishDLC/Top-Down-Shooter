using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Enemies.Boss.BossAttacks.Chessboard
{
    public class ChessboardAttack : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer chessboardSprite;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private ChessboardConfig config;

        private PlayerHealth _playerHealth;
        private CancellationToken _destroyCancellationToken;
        private ChessboardVisual _chessboardVisual;
        private bool _isPlayerInRange;
        private bool _canAttack = true;

        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _chessboardVisual = new ChessboardVisual(chessboardSprite, config);
            _chessboardVisual.OnAttackStarted += EnableHitCollider;
            _chessboardVisual.OnAttackEnded += DisableHitCollider;
            
            hitCollider.enabled = false;
            chessboardSprite.color = config.WarningTileColor;
            chessboardSprite.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            _chessboardVisual.OnAttackStarted -= EnableHitCollider;
            _chessboardVisual.OnAttackEnded -= DisableHitCollider;
        }
        //todo remove this stuff from update
        private void Update()
        {
            if (_isPlayerInRange)
            {
                DealDamage(_playerHealth, _destroyCancellationToken).Forget();
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = true;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = false;
            }
        }
        private async UniTask DealDamage(PlayerHealth playerHealth, CancellationToken token)
        {
            if (!_canAttack)
            {
                return;
            }

            _canAttack = false;
            playerHealth.TakeDamage(config.AttackDamage);
            await UniTask.Delay(TimeSpan.FromSeconds(config.AttackRate), cancellationToken: token);
            _canAttack = true;
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            return _chessboardVisual.ShowAttackWarningAsync(_destroyCancellationToken)
                .ContinueWith(() => _chessboardVisual.StartAttackAsync(_destroyCancellationToken));
        }
        private void EnableHitCollider()
        {
            hitCollider.enabled = true;
        }
        private void DisableHitCollider()
        {
            hitCollider.enabled = false;
            _isPlayerInRange = false;
        }
    }
}
