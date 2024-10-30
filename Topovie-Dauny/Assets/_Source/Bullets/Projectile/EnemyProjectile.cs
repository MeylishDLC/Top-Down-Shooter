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
        public Vector3 ProjectileMoveDir => _calculations.ProjectileMoveDir;
        public float NextYTrajectoryPosition => _calculations.NextYTrajectoryPosition;
        public float NextPositionYCorrectionAbsolute => _calculations.NextPositionYCorrectionAbsolute;
        
        [field:SerializeField] public float Lifetime {get; private set;}
        [SerializeField] private ProjectileVisual projectileVisual;
        
        private readonly ProjectileCalculations _calculations = new();
        private CancellationTokenSource _cancelDisableCts = new();
        private void OnEnable()
        {
            DisableAfterDelay(_cancelDisableCts.Token).Forget();
        }
        private void OnDisable()
        {
            OnObjectDisabled?.Invoke(this);
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CancelDisableRecreateCts();
                gameObject.SetActive(false);
            }
        }
        protected override void OnTriggerStay2D(Collider2D other)
        {
            base.OnTriggerStay2D(other);
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CancelDisableRecreateCts();
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
        private async UniTask DisableAfterDelay(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Lifetime), cancellationToken: token);
            gameObject.SetActive(false);
        }
        private void CancelDisableRecreateCts()
        {
            _cancelDisableCts?.Cancel();
            _cancelDisableCts?.Dispose();
            _cancelDisableCts = new CancellationTokenSource();
        }
    }
}