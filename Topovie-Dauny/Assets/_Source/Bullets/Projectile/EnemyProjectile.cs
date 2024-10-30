using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using Unity.VisualScripting;
using UnityEngine;

namespace Bullets.Projectile
{
    public class EnemyProjectile: EnemyAttack, IPoolObject<EnemyProjectile>
    {
        public event Action<EnemyProjectile> OnObjectDisabled;
        public Vector3 ProjectileMoveDir => _calculations.ProjectileMoveDir;
        public float NextYTrajectoryPosition => _calculations.NextYTrajectoryPosition;
        public float NextPositionYCorrectionAbsolute => _calculations.NextPositionYCorrectionAbsolute;
        
        [field:SerializeField] public float Lifetime {get; private set;}
        [SerializeField] private ProjectileVisual projectileVisual;
        
        private readonly ProjectileCalculations _calculations = new();
        private void OnEnable()
        {
            _calculations.OnDestinationReached += DisableOnDistanceReached;
        }
        private void OnDisable()
        {
            _calculations.OnDestinationReached -= DisableOnDistanceReached;
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
            _calculations.Initialize(target, transform, config, projectileVisual);
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