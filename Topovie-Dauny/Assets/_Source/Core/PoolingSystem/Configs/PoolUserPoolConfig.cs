using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.PoolingSystem.Configs
{
    [CreateAssetMenu(fileName = "New Pool User Pool Config", menuName = "Core/Pools/New Pool User Pool Config")]
    public class PoolUserPoolConfig: PoolConfig
    {
        [field:SerializeField] public string BulletPrefabAssetSimplifiedName { get; private set; }
    }
}