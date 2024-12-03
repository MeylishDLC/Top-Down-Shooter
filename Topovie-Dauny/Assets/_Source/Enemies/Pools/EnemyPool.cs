using Core.PoolingSystem;
using Core.PoolingSystem.Configs;
using UnityEngine;
using Zenject;

namespace Enemies.Pools
{
    public class EnemyPool: GenericPool<EnemyHealth>
    {
        protected readonly SceneContext ProjectContext;
        public EnemyPool(PoolConfig poolConfig, Transform parentTransform) : base(poolConfig, parentTransform)
        {
            ProjectContext = Object.FindFirstObjectByType<SceneContext>();
        }
        public override bool TryGetFromPool(out EnemyHealth instance)
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
        protected override EnemyHealth InstantiateNewObject()
        {
            var instance = ProjectContext.Container.InstantiatePrefabForComponent<EnemyHealth>(ObjectPrefab, ParentTransform);
            instance.gameObject.SetActive(false);
            instance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(instance);
            return instance;
        }
    }
}