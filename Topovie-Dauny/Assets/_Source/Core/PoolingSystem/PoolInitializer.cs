using System;
using System.Collections.Generic;
using System.Linq;
using Bullets.BulletPools;
using Enemies;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Core.PoolingSystem
{
    public class PoolInitializer
    {
        private readonly List<BulletPool> _bulletPoolsForPlayerWeapon = new();
        private readonly Dictionary<string, EnemyBulletPool> _bulletPoolsForEnemies = new();
        private readonly Dictionary<string, ProjectilePool> _projectilePoolsForEnemies = new();
        private readonly Dictionary<string, EnemyPool> _enemyPools = new(); 

        private readonly PoolConfig[] _bulletsForPlayerWeaponConfigs;
        private readonly PoolConfig[] _bulletsForEnemiesConfigs;
        private readonly PoolConfig[] _projectilesForEnemiesConfigs;
        private readonly PoolConfig[] _enemiesPoolsConfigs;

        public PoolInitializer(PoolInitializerConfig poolInitializerConfig)
        {
            _bulletsForPlayerWeaponConfigs = poolInitializerConfig.BulletsForPlayerWeaponConfigs;
            _bulletsForEnemiesConfigs = poolInitializerConfig.BulletsForEnemiesConfigs;
            _projectilesForEnemiesConfigs = poolInitializerConfig.ProjectilesForEnemiesConfigs;
            _enemiesPoolsConfigs = poolInitializerConfig.EnemiesPoolsConfigs;
        }
        public BulletPool GetBulletPoolForPlayerWeapon(int weaponNumber)
        {
            if (weaponNumber <= 0)
            {
                throw new Exception("Weapon number must be greater than 0");
            }

            if (weaponNumber > _bulletPoolsForPlayerWeapon.Count)
            {
                throw new Exception(
                    $"Pool fow weapon number {weaponNumber} was not initialized. Initialize it in config.");
            }

            return _bulletPoolsForPlayerWeapon.ElementAt(weaponNumber-1);
        }
        public EnemyBulletPool GetBulletPoolForEnemy(string prefabAssetName)
        {
            if (!IsAssetWithNameExists(prefabAssetName))
            {
                throw new Exception($"Prefab asset with name {prefabAssetName} doesn't exist");
            }

            return _bulletPoolsForEnemies[prefabAssetName];
        }
        public ProjectilePool GetProjectilePoolForEnemy(string prefabAssetName)
        {
            if (!IsAssetWithNameExists(prefabAssetName))
            {
                throw new Exception($"Prefab asset with name {prefabAssetName} doesn't exist");
            }

            return _projectilePoolsForEnemies[prefabAssetName];
        }
        public EnemyPool GetEnemyPool(string prefabAssetName)
        {
            if (!IsAssetWithNameExists(prefabAssetName))
            {
                throw new Exception($"Prefab asset with name {prefabAssetName} doesn't exist");
            }

            return _enemyPools[prefabAssetName];
        }
        public void InitAll()
        {
            if (_bulletsForPlayerWeaponConfigs.Any())
            {
                InitPlayerBulletPools();
            }
            if (_bulletsForEnemiesConfigs.Any())
            {
                InitEnemyBulletPools();
            }
            if (_projectilesForEnemiesConfigs.Any())
            {
                InitEnemyProjectilePools();
            }
            if (_enemiesPoolsConfigs.Any())
            {
                InitEnemyPools();
            }
        }
        public void CleanUp()
        {
            CleanUpPlayerBulletPools();
            CleanUpEnemyBulletPools();
            CleanUpEnemyProjectilePools();
            CleanUpEnemyPools();
        }
        private void CleanUpPlayerBulletPools()
        {
            if (!_bulletPoolsForPlayerWeapon.Any()) return;
            foreach (var pool in _bulletPoolsForPlayerWeapon)
            {
                pool.CleanUp();
            }
        }
        private void CleanUpEnemyBulletPools()
        {
            if (!_bulletPoolsForEnemies.Any()) return;
            foreach (var pool in _bulletPoolsForEnemies.Values)
            {
                pool.CleanUp();
            }
        }
        private void CleanUpEnemyProjectilePools()
        {
            if (!_projectilePoolsForEnemies.Any()) return;
            foreach (var pool in _projectilePoolsForEnemies.Values)
            {
                pool.CleanUp();
            } 
        }
        private void CleanUpEnemyPools()
        {
            if (!_enemyPools.Any()) return;
            foreach (var pool in _enemyPools.Values)
            {
                pool.CleanUp();
            }
        }
        private void InitPlayerBulletPools()
        {
            for (var i = 0; i < _bulletsForPlayerWeaponConfigs.Length; i++)
            {
                var parent = new GameObject($"{i + 1}WeaponBullets");
                var pool = new BulletPool(_bulletsForPlayerWeaponConfigs[i], parent.transform);
                _bulletPoolsForPlayerWeapon.Add(pool);
            }
        }
        private void InitEnemyBulletPools()
        {
            foreach (var config in _bulletsForEnemiesConfigs)
            {
                var prefabAssetName = config.PrefabAssetSimplifiedName;
                var parent = new GameObject($"EnemyProjectiles-{prefabAssetName}");
                var pool = new EnemyBulletPool(config, parent.transform);
                _bulletPoolsForEnemies.Add(prefabAssetName, pool);
            }
        }
        private void InitEnemyProjectilePools()
        {
            foreach (var config in _projectilesForEnemiesConfigs)
            {
                var prefabAssetName = config.PrefabAssetSimplifiedName;
                var parent = new GameObject($"EnemyProjectiles-{prefabAssetName}");
                var pool = new ProjectilePool(config, parent.transform);
                _projectilePoolsForEnemies.Add(prefabAssetName, pool);
            }
        }
        private void InitEnemyPools()
        {
            foreach (var config in _enemiesPoolsConfigs)
            {
                var prefabAssetName = config.PrefabAssetSimplifiedName;
                var parent = new GameObject($"Enemies-{prefabAssetName}");
                var pool = new EnemyPool(config, parent.transform);
                _enemyPools.Add(prefabAssetName, pool);
            }
        }
        private bool IsAssetWithNameExists(string assetName)
        {
            foreach (var locator in Addressables.ResourceLocators)
            {
                if (locator.Locate(assetName, typeof(object), out var locations))
                {
                    return true;
                }
            }
            return false;
        }
    }
}