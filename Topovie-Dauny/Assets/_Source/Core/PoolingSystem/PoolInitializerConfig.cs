using System;
using System.Collections.Generic;
using System.Linq;
using Bullets.BulletPools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Core.PoolingSystem
{
    [CreateAssetMenu(fileName = "New Pool Initializer Config", menuName = "Core/Pools/New Pool Initializer Config")]
    public class PoolInitializerConfig: ScriptableObject
    {
        [field:Header("BULLETS")] 
        [field:SerializeField] public PoolConfig[] BulletsForPlayerWeaponConfigs {get; private set;}
        [field:SerializeField] public PoolConfig[] BulletsForEnemiesConfigs {get; private set;}
        [field:SerializeField] public PoolConfig[] ProjectilesForEnemiesConfigs {get; private set;}
        
        [field:Header("ENEMIES")]
        [field:SerializeField] public PoolConfig[] EnemiesPoolsConfigs {get; private set;}
    }
}