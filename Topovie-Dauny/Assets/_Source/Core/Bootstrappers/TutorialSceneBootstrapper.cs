using System;
using System.Collections.Generic;
using Bullets;
using Core.PoolingSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Test_Gun;

namespace Core.Bootstrappers
{
    public class TutorialSceneBootstrapper: MonoBehaviour
    {
        [Header("GUNS")] 
        [SerializeField] private BasicGun pistolGun;
        [SerializeField] private BasicGun ppGun;
        
        [Header("POOLS")] 
        [SerializeField] private PoolConfigParentPair pistolBulletPoolDataPair; 
        [SerializeField] private PoolConfigParentPair ppBulletPoolDataPair;
        
        private BulletPool _pistolBulletPool;
        private BulletPool _ppBulletPool;
        private void Awake()
        {
            InitializePools();
            InitializeGuns();
        }
        private void OnDestroy()
        {
            CleanUpPools();
        }
        private void InitializePools()
        {
            _pistolBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, pistolBulletPoolDataPair.PoolParent);
            _ppBulletPool = new BulletPool(pistolBulletPoolDataPair.PoolConfig, ppBulletPoolDataPair.PoolParent);
        }
        private void InitializeGuns()
        {
            pistolGun.Initialize(_pistolBulletPool);
            ppGun.Initialize(_ppBulletPool);
        }

        private void CleanUpPools()
        {
            _pistolBulletPool.CleanUp();
            _ppBulletPool.CleanUp();
        }
    }
}