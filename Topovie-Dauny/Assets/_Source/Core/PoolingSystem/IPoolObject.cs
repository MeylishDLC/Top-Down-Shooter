using System;

namespace Core.PoolingSystem
{
    public interface IPoolObject<out T>
    {
        public event Action<T> OnObjectDisabled;
    }
}