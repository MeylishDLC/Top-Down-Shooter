using System;
using System.Linq;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cinemachine;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies;
using UnityEngine;

namespace Bullets
{
    public class RpgBullet: Bullet
    {
        private static readonly int Attack = Animator.StringToHash("attack");
        [SerializeField] private float blowupRange;
        [SerializeField] private float blowupDuration;
        [SerializeField] private Animator blowupAnimator;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private float impulseStrength;

        private SpriteRenderer _spriteRenderer;
        private Collider2D _col;
        private bool _isBlowingUp;
        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _col.enabled = true;
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                CancelRecreateCts();
                TriggerExplosion();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                CancelRecreateCts();

                var enemyHealth = other.gameObject.GetComponentInParent<IEnemyHealth>();
                enemyHealth.TakeDamage(damageAmount);
                
                TriggerExplosion();
            }
        }
        protected override void FixedUpdate()
        {
            if (!_isBlowingUp)
            {
                base.FixedUpdate();
            }
            else
            {
                Rb.velocity = Vector2.zero;
            }
        }
        private void TriggerExplosion()
        {
            _isBlowingUp = true;
            _col.enabled = false;
            _spriteRenderer.DOFade(0f, 0f);
            ExplodeAsync(CancellationToken.None).Forget();
        }
        private async UniTask ExplodeAsync(CancellationToken token)
        {
            var colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, blowupRange, colliders);
            
            var filteredColliders = colliders.Where(c => c != null).ToArray();
            
            foreach (var hitCollider in filteredColliders)
            {
                if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (hitCollider.gameObject.TryGetComponent(out IEnemyHealth enemyHealth))
                    {
                        enemyHealth.TakeDamage(damageAmount);
                    }
                }
            }
            
            blowupAnimator.SetTrigger(Attack);
            impulseSource.GenerateImpulse(impulseStrength);
            
            await UniTask.Delay(TimeSpan.FromSeconds(blowupDuration), cancellationToken: token);
            _isBlowingUp = false;
            gameObject.SetActive(false);
            _spriteRenderer.DOFade(1f, 0f);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blowupRange);
        }
    }
}