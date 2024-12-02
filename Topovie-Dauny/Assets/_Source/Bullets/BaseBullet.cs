using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseBullet: MonoBehaviour
    {
        protected Rigidbody2D Rb;

        [SerializeField] protected int damageAmount;
        [SerializeField] private float lifetime;
        [Range(1, 10)] [SerializeField] private float speed;

        private CancellationTokenSource _cancelDisableCts = new();
        protected virtual void Start()
        {
            Rb = GetComponent<Rigidbody2D>();
        }
        protected virtual void OnEnable()
        {
            DisableAfterTime(_cancelDisableCts.Token).Forget();
        }
        private void OnDestroy()
        {
            _cancelDisableCts?.Cancel();
            _cancelDisableCts?.Dispose();
        }
        protected virtual void FixedUpdate()
        {
            Rb.velocity = transform.right * speed;
        }
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                CancelRecreateCts();
                gameObject.SetActive(false);
            }
        }
        public void ChangeAttributes(float newLifetime, float newSpeed, int newDamageAmount)
        {
            lifetime = newLifetime;
            speed = newSpeed;
            damageAmount = newDamageAmount;
        }
        protected void CancelRecreateCts()
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