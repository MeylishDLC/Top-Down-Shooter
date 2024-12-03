using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.PoolingSystem.Configs
{
    [CreateAssetMenu(fileName = "New Pool Config", menuName = "Core/Pools/New Pool Config")]
    public class PoolConfig: ScriptableObject
    {
        [field: SerializeField] public int InitialPoolSize { get; private set; }
        [field: SerializeField] public int MaxPoolSize { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PrefabAsset { get; private set; }
        [field: SerializeField] public string PrefabAssetSimplifiedName { get; private set; }
    }
}