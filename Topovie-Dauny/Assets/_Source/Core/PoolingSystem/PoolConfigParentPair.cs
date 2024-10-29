using UnityEngine;

namespace Core.PoolingSystem
{
    [System.Serializable]
    public class PoolConfigParentPair
    {
        [field:SerializeField] public PoolConfig PoolConfig { get; private set; }
        [field:SerializeField] public Transform PoolParent { get; private set; }
    }
}