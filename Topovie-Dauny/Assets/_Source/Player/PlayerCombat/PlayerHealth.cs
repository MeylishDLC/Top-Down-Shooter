using System;
using System.Threading;
using Analytics;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerCombat
{
    //todo remove MonoBehaviour?
    public class PlayerHealth : MonoBehaviour
    {
        public event Action<float> OnDamageTaken;
        public event Action<float> OnHeal;
        public event Action OnDeath;
        public float CurrentHealth { get; private set; }
        public bool IsKnockedBack { get; private set; }
        public float MaxHealth { get; private set; }
        
        private PlayerConfig _playerConfig;
        
        private bool _canTakeDamage = true;
        private SpriteRenderer _spriteRenderer;
        private KnockBack _knockBack;
        private AudioManager _audioManager;
        private CancellationToken _deathCancellationToken;

        [Inject]
        public void Construct(AudioManager audioManager, PlayerConfig config)
        {
            _audioManager = audioManager;
            _playerConfig = config;
        }
        private void Awake()
        {
            _deathCancellationToken = this.GetCancellationTokenOnDestroy();
            
            MaxHealth = _playerConfig.MaxHealth;
            CurrentHealth = MaxHealth;

            //todo bind instead of create
            _knockBack = new KnockBack
                (this,GetComponent<Rigidbody2D>(), _playerConfig.KnockbackTime, _playerConfig.KnockbackThrust);
            _knockBack.OnKnockBackStarted += StartKnockback;
            _knockBack.OnKnockBackEnded += EndKnockback;
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
            await UniTask.Delay(TimeSpan.FromSeconds(_playerConfig.InvincibilityTime), cancellationToken: token);
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
