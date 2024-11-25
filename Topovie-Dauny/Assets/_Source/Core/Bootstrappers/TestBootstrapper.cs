using System.Threading;
using Bullets.BulletPatterns;
using Bullets.BulletPools;
using Core.LoadingSystem;
using Core.PoolingSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Bootstrappers
{
    public class TestBootstrapper: MonoBehaviour
    {
        [SerializeField] private BulletSpawner[] bulletSpawners;
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair enemyBulletPoolDataPair;

        private EnemyBulletPool _enemyBulletPool;
        private void Awake()
        {
            InitializePools();
            InitializeTowers();
        }
        private void OnDestroy()
        {
            CleanUpPools();
        }
        public void InitializePools()
        {
            _enemyBulletPool =
                new EnemyBulletPool(enemyBulletPoolDataPair.PoolConfig, enemyBulletPoolDataPair.PoolParent);
        }
        public void InitializeTowers()
        {
            foreach (var spawner in bulletSpawners)
            {
                spawner.Construct(_enemyBulletPool);
            }
        }
        public void CleanUpPools()
        {
            _enemyBulletPool.CleanUp();
        }
    }
}