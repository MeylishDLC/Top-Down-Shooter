using System;
using System.Linq;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Enemies;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Bullets
{
    public class RpgBullet: Bullet
    {
        [SerializeField] private float blowupRange;
        [SerializeField] private float blowupDuration;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private float impulseStrength;

        private AudioManager _audioManager;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _col;
        private bool _isBlowingUp;
        private CancellationToken _ctOnDestroy;
        
        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        private void Awake()
        {
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();
            _col = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        protected override void OnEnable()
        {
            //shooting bullet
            base.OnEnable();
            _col.enabled = true;
            _spriteRenderer.DOFade(1f, 0f).ToUniTask(cancellationToken: _ctOnDestroy).Forget();
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
            _spriteRenderer.DOFade(0f, 0f).ToUniTask(cancellationToken: _ctOnDestroy).Forget();
            ExplodeAsync(_ctOnDestroy).Forget();
        }
        private async UniTask ExplodeAsync(CancellationToken token)
        {
            AttackEnemiesInRange();
            
            _audioManager.PlayOneShot(_audioManager.FMODEvents.RpgBlowUpSound);
            impulseSource.GenerateImpulse(impulseStrength);
            
            await UniTask.Delay(TimeSpan.FromSeconds(blowupDuration), cancellationToken: token);
            _isBlowingUp = false;
            gameObject.SetActive(false);
        }
        private void AttackEnemiesInRange()
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, blowupRange);
            var filteredColliders = hitColliders.Where(c => c != null).ToArray();

            foreach (var col in filteredColliders)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (col.gameObject.transform.parent.TryGetComponent<IEnemyHealth>(out var enemyHealth))
                    {
                        enemyHealth.TakeDamage(damageAmount);
                    }
                }
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blowupRange);
        }
    }
}