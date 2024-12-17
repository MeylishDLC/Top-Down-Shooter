using System.Linq;
using Core.PoolingSystem;
using Core.PoolingSystem.Configs;
using UnityEngine;
using Zenject;

namespace Bullets.BulletPools
{
    public class BulletPool: GenericPool<Bullet>
    {
        protected readonly SceneContext ProjectContext;
        public BulletPool(PoolConfig poolConfig, Transform parentTransform)
            : base(poolConfig, parentTransform)
        {
            ProjectContext = Object.FindFirstObjectByType<SceneContext>();
        }
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
            var bulletInstance = ProjectContext.Container.InstantiatePrefabForComponent<Bullet>(ObjectPrefab, ParentTransform);
            bulletInstance.gameObject.SetActive(false);
            bulletInstance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(bulletInstance);
            return bulletInstance;
        }
    }
}