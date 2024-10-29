using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies;
using UnityEngine;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour, IPoolObject<Bullet>
    {
        public event Action<Bullet> OnObjectDisabled;
        
        [SerializeField] protected int damageAmount;
        [SerializeField] private float lifetime;
        [Range(1, 10)] [SerializeField] private float speed;

        private Rigidbody2D _rb;
        private CancellationTokenSource _cancelDisableCts = new();
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        private void OnEnable()
        {
            DisableAfterTime(_cancelDisableCts.Token).Forget();
        }
        private void OnDisable()
        {
            OnObjectDisabled?.Invoke(this);
        }
        private void OnDestroy()
        {
            _cancelDisableCts?.Cancel();
            _cancelDisableCts?.Dispose();
        }
        private void FixedUpdate()
        {
            _rb.velocity = transform.right * speed;
        }
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                CancelDisableRecreateCts();
                gameObject.SetActive(false);
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
                enemyHealth.TakeDamage(damageAmount);
                
                CancelDisableRecreateCts();
                gameObject.SetActive(false);
            }
        }
        protected void CancelDisableRecreateCts()
        {
            _cancelDisableCts?.Cancel();
            _cancelDisableCts?.Dispose();
            _cancelDisableCts = new CancellationTokenSource();
        }
        private async UniTask DisableAfterTime(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(lifetime), cancellationToken: token);
            gameObject.SetActive(false);
        }
    }
}
