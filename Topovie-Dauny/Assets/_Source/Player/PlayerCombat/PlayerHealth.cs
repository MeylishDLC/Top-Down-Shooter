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
        
        public void TakeDamage(int damageAmount)
        {
            if (!_canTakeDamage)
            {
                return;
            }
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

        public void SetCanTakeDamage(bool canTakeDamage)
        {
            _canTakeDamage = canTakeDamage;
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
