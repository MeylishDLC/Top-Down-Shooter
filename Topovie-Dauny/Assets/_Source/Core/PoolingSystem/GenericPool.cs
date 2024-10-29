using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.PoolingSystem
{
    public abstract class GenericPool<T>: IPool<T> where T : Object, IPoolObject<T>
    {
        protected readonly List<T> AllObjects = new();
        protected readonly Queue<T> Pool = new();
        protected readonly int MaxPoolSize;
        protected readonly T ObjectPrefab;
        protected readonly Transform ParentTransform;
        
        private readonly int _startPoolSize;
        private readonly string _poolName;
        protected GenericPool(PoolConfig poolConfig, Transform parentTransform)
        {
            _poolName = poolConfig.PoolName;
            _startPoolSize = poolConfig.InitialPoolSize;
            MaxPoolSize = poolConfig.MaxPoolSize;
            ObjectPrefab = poolConfig.Prefab.GetComponent<T>();
            ParentTransform = parentTransform;

            InitPool(ObjectPrefab);
        }
        public void InitPool(T prefab)
        {
            for (int i = 0; i < _startPoolSize; i++)
            {
                var bulletInstance = InstantiateNewObject();
                Pool.Enqueue(bulletInstance);
            }
        }
        public abstract bool TryGetFromPool(out T instance);
        public void ReturnToPool(T instance)
        {
            Pool.Enqueue(instance);
        }
        public void CleanUp()
        {
            foreach (var instance in AllObjects)
            {
                instance.OnObjectDisabled -= ReturnToPool;
            }
        }
        protected abstract T InstantiateNewObject();
    }
}