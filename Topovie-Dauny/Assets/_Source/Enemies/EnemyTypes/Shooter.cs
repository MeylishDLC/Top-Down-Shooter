using System;
using Bullets.BulletPools;
using Bullets.Projectile;
using Core.PoolingSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.EnemyTypes
{
    public class Shooter: MonoBehaviour, IPoolUser
    {
        public event Action OnShootTriggered;

        [SerializeField] private Transform shootingPoint;
        [SerializeField] private float minShootRate;
        [SerializeField] private float maxShootRate;
        [SerializeField] private ProjectileConfig projectileConfig;
        
        private float _shootTimer;
        private Transform _target;
        private ProjectilePool _projectilePool;
        private void Start()
        {
            var health = GetComponent<EnemyHealth>();
            _target = health.PlayerMovement.transform;
        }
        public void InjectPool(ProjectilePool pool)
        {
            _projectilePool = pool;
        }
        private void Update()
        {
            _shootTimer -= Time.deltaTime;
            if (_shootTimer <= 0)
            {
                var shootRate = Random.Range(minShootRate, maxShootRate);
                _shootTimer = shootRate;
                OnShootTriggered?.Invoke();
            }
        }
        public void Shoot()
        {
            if (_projectilePool.TryGetFromPool(out var projectile))
            {
                projectile.transform.position = shootingPoint.position;
                projectile.Initialize(_target, projectileConfig);
            }
        }
    }
}