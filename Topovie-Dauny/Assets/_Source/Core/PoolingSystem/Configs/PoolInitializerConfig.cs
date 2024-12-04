using System.Collections.Generic;
using UnityEngine;

namespace Core.PoolingSystem.Configs
{
    [CreateAssetMenu(fileName = "New Pool Initializer Config", menuName = "Core/Pools/New Pool Initializer Config")]
    public class PoolInitializerConfig: ScriptableObject
    {
        [field:Header("BULLETS")] 
        [field:SerializeField] public PoolConfig[] BulletsForPlayerWeaponConfigs {get; private set;}
        [field:SerializeField] public PoolConfig[] BulletsForEnemiesConfigs {get; private set;}
        [field: SerializeField] public PoolConfig[] ProjectilesForEnemiesConfigs { get; private set; }
        
        [field:Header("ENEMIES")]
        [field:SerializeField] public PoolConfig[] EnemiesPoolsConfigs {get; private set;}
        [field: SerializeField] public List<PoolUserPoolConfig> PoolUserEnemiesPoolsConfigs { get; private set; } = new();
    }
}