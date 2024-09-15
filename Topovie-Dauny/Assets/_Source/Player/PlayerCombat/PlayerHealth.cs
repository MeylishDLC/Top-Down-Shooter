using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using UnityEngine;

namespace Player.PlayerCombat
{
    public class PlayerHealth : MonoBehaviour
    {
        public KnockBack KnockBack { get; private set; }

        [SerializeField] private int maxHealth;
        
        [Header("Knockback Settings")] 
        [SerializeField] private float knockbackThrust;
        [SerializeField] private int knockbackTimeMilliseconds;
        [SerializeField] private int invincibilityTime;
        
        private int _currentHealth;
        private bool _canTakeDamage = true;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            KnockBack = new KnockBack
                (GetComponent<Rigidbody2D>(), knockbackTimeMilliseconds, knockbackThrust);
        }
        private void Start()
        {
            _currentHealth = maxHealth;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && _canTakeDamage)
            {
                //todo: replace this one with actual atk
                TakeDamage(3);
                KnockBack.GetKnockedBack(other.gameObject.transform);
            }
        }
        private void TakeDamage(int damageAmount)
        {
            _canTakeDamage = false;
            _currentHealth -= damageAmount;
            RecoverFromDamageAsync(CancellationToken.None).Forget();
        }
        private async UniTask RecoverFromDamageAsync(CancellationToken token)
        {
            await UniTask.Delay(invincibilityTime, cancellationToken: token);
            _canTakeDamage = true;
        }

        private void CheckIfDead()
        {
            //todo: play death animation and show game over screen 
            if (_currentHealth <= 0)
            {
                
            }
        }
    }
}
