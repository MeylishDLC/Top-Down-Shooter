using Bullets.BulletPools;
using Core.PoolingSystem;
using Enemies;
using UnityEngine;

namespace Bullets.BulletPatterns
{
    public class BulletSpawner: MonoBehaviour, IPoolUser
    {
        [SerializeField] private BulletSpawnerConfig config;

        private EnemyBulletPool _enemyBulletPool;
        private float _timer;
        public void InjectPool(EnemyBulletPool pool)
        {
            _enemyBulletPool = pool;
        }
        private void Update()
        {
            _timer += Time.deltaTime;
            if (config.SpawnerType == SpawnerType.Spin)
            {
                transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + config.SpinSpeed * Time.deltaTime);
            }
            if (_timer >= config.FiringRate)
            {
                Fire();
                _timer = 0;
            }
        }
        private void Fire()
        {
            if (_enemyBulletPool.TryGetFromPool(out var bullet))
            {
                bullet.transform.position = transform.position;
                bullet.ChangeAttributes(config.Lifetime, config.Speed, config.Damage);
                bullet.transform.rotation = transform.rotation;
            } 
        }
        
    }
}