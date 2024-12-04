using System.Linq;
using Core.PoolingSystem;
using Core.PoolingSystem.Configs;
using UnityEngine;

namespace Bullets.BulletPools
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
        public override void DisableAll()
        {
            foreach (var bullet in AllObjects.Where(bullet => bullet.gameObject.activeSelf))
            {
                bullet.gameObject.SetActive(false);
                ReturnToPool(bullet);
            }
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