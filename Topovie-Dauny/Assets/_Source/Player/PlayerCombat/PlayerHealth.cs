using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerHealth : MonoBehaviour
    {
        public event Action<float> OnDamageTaken;
        public event Action<float> OnHeal;
        public event Action OnDeath;
        public float CurrentHealth { get; private set; }
        public bool IsKnockedBack { get; private set; }
        [field:SerializeField] public float MaxHealth { get; private set; }
        
        [Header("Knockback Settings")] 
        [SerializeField] private float knockbackThrust;
        [SerializeField] private int knockbackTimeMilliseconds;
        [SerializeField] private int invincibilityTime;
        
        private bool _canTakeDamage = true;
        private SpriteRenderer _spriteRenderer;
        private KnockBack _knockBack;
        private AudioManager _audioManager;
        private CancellationToken _deathCancellationToken;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        private void Awake()
        {
            _deathCancellationToken = this.GetCancellationTokenOnDestroy();
            _knockBack = new KnockBack
                (this,GetComponent<Rigidbody2D>(), knockbackTimeMilliseconds, knockbackThrust);
            _knockBack.OnKnockBackStarted += StartKnockback;
            _knockBack.OnKnockBackEnded += EndKnockback;
        }
        private void Start()
        {
            CurrentHealth = MaxHealth;
        }
        private void OnDestroy()
        {
            _knockBack.OnKnockBackStarted -= StartKnockback;
            _knockBack.OnKnockBackEnded -= EndKnockback;
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
            
            RecoverFromDamageAsync(_deathCancellationToken).Forget();
            CheckIfDead();
        }
        public void TakeDamageWithKnockback(int damageAmount, Transform damageSource)
        {
            if (!_canTakeDamage)
            {
                return;
            }
            _canTakeDamage = false;
            CurrentHealth -= damageAmount;
            OnDamageTaken?.Invoke(damageAmount);
            _audioManager.PlayOneShot(_audioManager.FMODEvents.PlayerHitSound);
            _knockBack.GetKnockedBack(damageSource);
            
            RecoverFromDamageAsync(_deathCancellationToken).Forget();
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
            CheckIfDead();
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
        private void StartKnockback() => IsKnockedBack = true;
        private void EndKnockback() => IsKnockedBack = false;
        private void CheckIfDead()
        {
            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}
