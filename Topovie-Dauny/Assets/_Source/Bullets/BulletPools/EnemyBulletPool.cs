using Core.PoolingSystem;
using UnityEngine;
using Zenject;

namespace Bullets.BulletPools
{
    public class EnemyBulletPool: GenericPool<EnemyBullet>
    {
        public EnemyBulletPool(PoolConfig poolConfig, Transform parentTransform) : base(poolConfig, parentTransform) { }

        public override bool TryGetFromPool(out EnemyBullet instance)
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
        protected override EnemyBullet InstantiateNewObject()
        {
            var bulletInstance = Object.Instantiate(ObjectPrefab, ParentTransform);
            bulletInstance.gameObject.SetActive(false);
            bulletInstance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(bulletInstance);
            return bulletInstance;
        }
    }
}