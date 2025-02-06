using System;
using Enemies;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Interactable.HealingStuff
{
    public class HealOrbsSpawner
    {
        private readonly SceneContext _sceneContext;
        private readonly int _dropPercentChance;
        private readonly GameObject _orbPrefab;
        
        public HealOrbsSpawner(HealingOrbsSpawnerConfig config)
        {
            if (!config.OrbPrefab)
            {
                throw new Exception("OrbPrefab is null");
            }
            _orbPrefab = config.OrbPrefab;
           
            _sceneContext = Object.FindFirstObjectByType<SceneContext>();
            _dropPercentChance = config.SpawnChancePercent;

            EnemyMovement.OnEnemyDisabled += SpawnOrb;
        }
        public void CleanUp()
        {
            EnemyMovement.OnEnemyDisabled -= SpawnOrb;
        }
        private bool CanSpawn()
        {
            return Random.Range(0, 100) < _dropPercentChance;
        }

        private void SpawnOrb(Vector3 spawnPosition)
        {
            if (!CanSpawn())
            {
                return;
            }
            _sceneContext.Container.InstantiatePrefabForComponent<HealOrb>
                (_orbPrefab, spawnPosition, Quaternion.identity, null);
            Debug.Log("Spawned orb");
        }
        
    }
}