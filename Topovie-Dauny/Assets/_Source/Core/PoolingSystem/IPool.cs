using Object = UnityEngine.Object;

namespace Core.PoolingSystem
{
    public interface IPool<T> where T : IPoolObject<T>
    {
        public void InitPool(T prefab);
        public bool TryGetFromPool(out T instance);
        public void ReturnToPool(T instance);
    }
}