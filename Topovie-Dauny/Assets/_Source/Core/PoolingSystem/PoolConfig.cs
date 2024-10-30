using UnityEngine;

namespace Core.PoolingSystem
{
    [CreateAssetMenu(fileName = "New Pool Config", menuName = "Pools/New Pool Config")]
    public class PoolConfig: ScriptableObject
    {
        [field: SerializeField] public int InitialPoolSize { get; private set; }
        [field: SerializeField] public int MaxPoolSize { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}