using Bullets.BulletPools;
using Bullets.Projectile;
using Player.PlayerControl;
using UnityEngine;

namespace Enemies
{
    public class Shooter: MonoBehaviour
    {
        [SerializeField] private float shootRate;
        [SerializeField] private ProjectileConfig projectileConfig;
        
        private float _shootTimer;
        private Transform _target;
        private ProjectilePool _projectilePool;
        public void Construct(PlayerMovement playerMovement)
        {
            _target = playerMovement.transform;
        }
        public void InitializePool(ProjectilePool projectilePool)
        {
            _projectilePool = projectilePool;
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