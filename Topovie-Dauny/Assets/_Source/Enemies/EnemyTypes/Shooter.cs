using System;
using Bullets.BulletPools;
using Bullets.Projectile;
using Core.PoolingSystem;
using Player.PlayerControl;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    public class Shooter: MonoBehaviour, IPoolUser
    {
        [SerializeField] private float shootRate;
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
                _shootTimer = shootRate;
                if (_projectilePool.TryGetFromPool(out var projectile))
                {
                    projectile.transform.position = transform.position;
                    projectile.Initialize(_target, projectileConfig);
                }
            }
        }
        
    }
}