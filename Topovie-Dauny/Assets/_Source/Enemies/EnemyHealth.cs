using System;
using Core.PoolingSystem;
using Enemies.Combat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyHealth: MonoBehaviour, IEnemyHealth, IPoolObject<EnemyHealth>
    {
        public event Action<EnemyHealth> OnObjectDisabled;
        public event Action OnEnemyDied;
        public event Action OnDamageTaken;
        public PlayerMovement PlayerMovement {get; private set;}
        
        [SerializeField] private EnemyMovement enemyMovement;
        [SerializeField] private int maxHealth;
        
        private int _currentHealth;
        private bool _isDead;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            PlayerMovement = playerMovement;
            _currentHealth = maxHealth;
            enemyMovement.SetDestination(playerMovement.transform);
        }
        private void OnEnable()
        {
            _isDead = false;
            _currentHealth = maxHealth;
        }
        private void OnDisable()
        {
            OnObjectDisabled?.Invoke(this);
        }
        public void TakeDamage(int damage)
        {
            if (_isDead)
            {
                return;
            }
            
            _currentHealth -= damage;
            OnDamageTaken?.Invoke();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }
        private void Die()
        {
            _isDead = true;
            OnEnemyDied?.Invoke();
        }
    }
}