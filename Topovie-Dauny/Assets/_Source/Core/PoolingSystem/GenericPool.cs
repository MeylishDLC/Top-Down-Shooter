using System;
using System.Collections.Generic;
using System.Threading;
using Core.LoadingSystem;
using Core.PoolingSystem.Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Core.PoolingSystem
{
    public abstract class GenericPool<T>: IPool<T> where T : Object, IPoolObject<T>
    {
        protected readonly List<T> AllObjects = new();
        protected readonly Queue<T> Pool = new();
        protected readonly int MaxPoolSize;
        protected readonly Transform ParentTransform;
        protected T ObjectPrefab;
        
        private readonly int _startPoolSize;
        private AssetLoader _assetLoader;
        protected GenericPool(PoolConfig poolConfig, Transform parentTransform)
        {
            _startPoolSize = poolConfig.InitialPoolSize;
            ParentTransform = parentTransform;
            MaxPoolSize = poolConfig.MaxPoolSize;
            LoadPrefabAsset(poolConfig.PrefabAsset, CancellationToken.None)
                .ContinueWith(() => InitPool(ObjectPrefab)).Forget(); 
        }
        public void InitPool(T prefab)
        {
            for (int i = 0; i < _startPoolSize; i++)
            {
                var instance = InstantiateNewObject();
                Pool.Enqueue(instance);
            }
        }
        public abstract bool TryGetFromPool(out T instance);
        public void ReturnToPool(T instance)
        {
            Pool.Enqueue(instance);
        }
        public void CleanUp()
        {
            _assetLoader.ReleaseStoredInstance();
            foreach (var instance in AllObjects)
            {
                instance.OnObjectDisabled -= ReturnToPool;
            }
        }
        protected abstract T InstantiateNewObject();
        private async UniTask LoadPrefabAsset(AssetReferenceGameObject prefabAsset, CancellationToken token)
        {
            var handle = prefabAsset.Asset as GameObject;
                
            if (handle == null)
            {
                _assetLoader = new AssetLoader();
                var gameObject = await _assetLoader.LoadGameObject(prefabAsset, token);
                ObjectPrefab = gameObject.GetComponent<T>();
            }
            else
            {
                ObjectPrefab = handle.GetComponent<T>();
            }
        }
       
    }
}