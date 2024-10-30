using Player.PlayerControl;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bullets.Projectile
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
                    projectile.Initialize(_target, projectileConfig);
                }
            }
        }
    }
}