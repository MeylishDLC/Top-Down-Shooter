using System.Collections.Generic;
using Core.PoolingSystem;
using UnityEngine;

namespace Bullets
{
    public class BulletPool: GenericPool<Bullet>
    {
        public BulletPool(PoolConfig poolConfig, Transform parentTransform) : base(poolConfig, parentTransform) { }

        public override bool TryGetFromPool(out Bullet instance)
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
        protected override Bullet InstantiateNewObject()
        {
            var bulletInstance = Object.Instantiate(ObjectPrefab, ParentTransform);
            bulletInstance.gameObject.SetActive(false);
            bulletInstance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(bulletInstance);
            return bulletInstance;
        }
    }
}