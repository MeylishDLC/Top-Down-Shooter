using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using UnityEngine;

namespace Bullets.Projectile
{
    public class EnemyProjectile: EnemyAttack, IPoolObject<EnemyProjectile>
    {
        public event Action<EnemyProjectile> OnObjectDisabled;
        [field:SerializeField] public float Lifetime {get; private set;}
        
        private readonly ProjectileCalculations _calculations = new();
        private void OnEnable()
        {
            _calculations.OnDestinationReached += DisableOnDistanceReached;
        }
        private void OnDisable()
        {
            _calculations.OnDestinationReached -= DisableOnDistanceReached;
            OnObjectDisabled?.Invoke(this);
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                gameObject.SetActive(false);
            }
        }
        protected override void OnTriggerStay2D(Collider2D other)
        {
            base.OnTriggerStay2D(other);
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                gameObject.SetActive(false);
            }
        }
        private void Update()
        {
            UpdatePosition();
        }
        public void Initialize(Transform target, ProjectileConfig config)
        {
            _calculations.Initialize(target, transform, config);
        }
        private void UpdatePosition()
        {
            _calculations.UpdateProjectilePosition(transform);
        }
        private void DisableOnDistanceReached()
        {
            DisableOnDistanceReachedAsync(CancellationToken.None).Forget();
        }
        private async UniTask DisableOnDistanceReachedAsync(CancellationToken token)
        {
            _calculations.OnDestinationReached -= DisableOnDistanceReached;
            await UniTask.Delay(TimeSpan.FromSeconds(Lifetime), cancellationToken: token);
            gameObject.SetActive(false);
        }
    }
}