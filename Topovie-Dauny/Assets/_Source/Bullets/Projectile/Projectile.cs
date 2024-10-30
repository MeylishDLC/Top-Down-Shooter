using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies.Combat;
using Unity.VisualScripting;
using UnityEngine;

namespace Bullets.Projectile
{
    public class Projectile : MonoBehaviour
    {
        public readonly ProjectileCalculations Calculations = new();
        [field:SerializeField] public float Lifetime {get; private set;}
        [SerializeField] private ProjectileVisual projectileVisual;
        private void Awake()
        {
            Calculations.OnDestinationReached += DestroyOnDistanceReached;
        }
        private void Update()
        {
            UpdatePosition();
        }
        public void Initialize(Transform target, ProjectileConfig config)
        {
            Calculations.Initialize(target, transform, config, projectileVisual);
        }
        private void UpdatePosition()
        {
            Calculations.UpdateProjectilePosition(transform);
        }
        private void DestroyOnDistanceReached()
        {
            Calculations.OnDestinationReached -= DestroyOnDistanceReached;
            DestroyOnDistanceReachedAsync(CancellationToken.None).Forget();
        }

        private async UniTask DestroyOnDistanceReachedAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Lifetime), cancellationToken: token);
            Destroy(gameObject);
        }
    }

}