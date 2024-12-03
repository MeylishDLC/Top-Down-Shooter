using Core.PoolingSystem;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyPool: GenericPool<EnemyHealth>
    {
        private readonly SceneContext _projectContext;
        public EnemyPool(PoolConfig poolConfig, Transform parentTransform) : base(poolConfig, parentTransform)
        {
            _projectContext = Object.FindFirstObjectByType<SceneContext>();
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
            var instance = _projectContext.Container.InstantiatePrefabForComponent<EnemyHealth>(ObjectPrefab, ParentTransform);
            instance.gameObject.SetActive(false);
            instance.OnObjectDisabled += ReturnToPool;
            AllObjects.Add(instance);
            return instance;
        }
    }
}