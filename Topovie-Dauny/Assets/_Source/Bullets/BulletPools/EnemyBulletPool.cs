using System;
using System.Linq;
using Core.PoolingSystem;
using Core.PoolingSystem.Configs;
using UnityEngine;
using Object = UnityEngine.Object;

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
        public override void DisableAll()
        {
            foreach (var bullet in AllObjects.Where(b => b.gameObject.activeSelf))
            {
                bullet.gameObject.SetActive(false);
                ReturnToPool(bullet);
            }
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