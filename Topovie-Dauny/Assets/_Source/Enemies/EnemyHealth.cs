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
        public KnockBack KnockBack { get; private set; }
        public PlayerMovement PlayerMovement {get; private set;}
        
        [SerializeField] private int maxHealth;
        
        [Header("Knockback Settings")]
        [SerializeField] private int knockBackTimeMilliseconds = 200;
        [SerializeField] private float knockbackThrust = 15f;
        
        private int _currentHealth;
        private bool _isDead;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            PlayerMovement = playerMovement;
            KnockBack = new KnockBack(this,GetComponent<Rigidbody2D>(), knockBackTimeMilliseconds, knockbackThrust);
        }
        private void Awake()
        {
            _currentHealth = maxHealth;
        }
        private void OnEnable()
        {
            _isDead = false;
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
            KnockBack.GetKnockedBack(PlayerMovement.transform);

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