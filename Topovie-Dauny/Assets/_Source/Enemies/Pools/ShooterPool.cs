using Bullets.BulletPools;
using Core.PoolingSystem;
using Core.PoolingSystem.Configs;
using Enemies.EnemyTypes;
using UnityEngine;

namespace Enemies.Pools
{
    public class ShooterPool: EnemyPool
    {
        private readonly ProjectilePool _projectilePool;
        public ShooterPool(PoolConfig poolConfig, Transform parentTransform, ProjectilePool projectilePool) : base(poolConfig, parentTransform)
        {
            _projectilePool = projectilePool;
        }
        protected override EnemyHealth InstantiateNewObject()
        {
            var instance = ProjectContext.Container.InstantiatePrefabForComponent<EnemyHealth>(ObjectPrefab, ParentTransform);
            InjectPool(instance);
            instance.gameObject.SetActive(false);
            instance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(instance);
            return instance;
        }
        private void InjectPool(EnemyHealth instance)
        {
            var shooter = instance.GetComponent<Shooter>();
            shooter.InjectPool(_projectilePool);
        }
    }
}