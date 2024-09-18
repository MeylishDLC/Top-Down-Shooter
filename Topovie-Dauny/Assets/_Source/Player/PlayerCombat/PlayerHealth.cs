using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using UnityEngine;

namespace Player.PlayerCombat
{
    public class PlayerHealth : MonoBehaviour
    {
        public event Action<float> OnDamageTaken;
        public event Action<float> OnHeal;
        public event Action OnDeath;
        public float CurrentHealth { get; private set; }
        [field:SerializeField] public float MaxHealth { get; private set; }
        public KnockBack KnockBack { get; private set; }
        
        [Header("Knockback Settings")] 
        [SerializeField] private float knockbackThrust;
        [SerializeField] private int knockbackTimeMilliseconds;
        [SerializeField] private int invincibilityTime;
        
        private bool _canTakeDamage = true;
        private SpriteRenderer _spriteRenderer;
        private void Awake()
        {
            KnockBack = new KnockBack
                (GetComponent<Rigidbody2D>(), knockbackTimeMilliseconds, knockbackThrust);
        }
        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("EnemyAttack") && _canTakeDamage)
            {
                if (!other.gameObject.TryGetComponent(out EnemyAttack enemyAttack))
                {
                    enemyAttack = other.gameObject.GetComponentInParent<EnemyAttack>();
                }
                TakeDamage(enemyAttack.Attack);
                KnockBack.GetKnockedBack(other.gameObject.transform);
                Destroy(other.gameObject);
            }
        }

        public void TakeDamage(int damageAmount)
        {
            _canTakeDamage = false;
            CurrentHealth -= damageAmount;
            OnDamageTaken?.Invoke(damageAmount);
            
            RecoverFromDamageAsync(CancellationToken.None).Forget();
            CheckIfDead();
        }

        public void Heal(int healAmount)
        {
            if (CurrentHealth > 0 && CurrentHealth < MaxHealth)
            {
                if (CurrentHealth + healAmount > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }
                else
                {
                    CurrentHealth += healAmount;
                }
                OnHeal?.Invoke(healAmount);
            }
        }
        private async UniTask RecoverFromDamageAsync(CancellationToken token)
        {
            await UniTask.Delay(invincibilityTime, cancellationToken: token);
            _canTakeDamage = true;
        }

        private void CheckIfDead()
        {
            //todo: play death animation and show game over screen 
            if (CurrentHealth <= 0)
            {
                //todo: pause whole game
                OnDeath?.Invoke();
            }
        }
    }
}
