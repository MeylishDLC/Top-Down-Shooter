using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Enemies.Boss.BossAttacks
{
    [RequireComponent(typeof(Collider2D))]
    public class ContinuousAttack: MonoBehaviour
    {
        [SerializeField] private Collider2D col;
        [SerializeField] private int damage;
        [SerializeField] private float attackRate;

        private CancellationToken _destroyCancellationToken;
        private PlayerHealth _playerHealth;
        private bool _isPlayerInRange;
        private bool _canAttack = true;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.GetComponent<PlayerHealth>();
        }
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
        }
        private void Update()
        {
            if (!col.enabled)
            {
                _isPlayerInRange = false;
            }
            
            if (_isPlayerInRange)
            {
                DealDamageAsync(_destroyCancellationToken).Forget();
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
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
        private async UniTask DealDamageAsync(CancellationToken token)
        {
            if (!_canAttack)
            {
                return;
            }

            _canAttack = false;
            _playerHealth.TakeDamage(damage);
            await UniTask.Delay(TimeSpan.FromSeconds(attackRate), cancellationToken: token);
            _canAttack = true;
        }
    }
}