using Bullets.Projectile;
using Core.PoolingSystem;
using UnityEngine;

namespace Bullets.BulletPools
{
    public class ProjectilePool: GenericPool<EnemyProjectile>
    {
        public ProjectilePool(PoolConfig poolConfig, Transform parentTransform) : base(poolConfig, parentTransform)
        { }

        public override bool TryGetFromPool(out EnemyProjectile instance)
        {
            if (Pool.TryDequeue(out instance))
            {
                instance.gameObject.SetActive(true);
                return true;
            }

            if (AllObjects.Count < MaxPoolSize)
            {
                instance = InstantiateNewObject();
                instance.gameObject.SetActive(true);
                return true;
            }

            instance = null;
            return false;
        }
        protected override EnemyProjectile InstantiateNewObject()
        {
            var instance = Object.Instantiate(ObjectPrefab, ParentTransform);
            instance.gameObject.SetActive(false);
            instance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(instance);
            return instance;
        }
    }
}