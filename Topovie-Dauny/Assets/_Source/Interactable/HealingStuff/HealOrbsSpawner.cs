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
        private readonly ProjectContext _projectContext;
        private readonly int _dropPercentChance;
        private readonly GameObject _orbPrefab;
        
        public HealOrbsSpawner(HealingOrbsSpawnerConfig config, ProjectContext projectContext)
        {
            if (!config.OrbPrefab)
            {
                throw new Exception("OrbPrefab is null");
            }
            _orbPrefab = config.OrbPrefab;
           
            _projectContext = projectContext;
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
            _projectContext.Container.InstantiatePrefabForComponent<HealOrb>
                (_orbPrefab, spawnPosition, Quaternion.identity, null);
            Debug.Log("Spawned orb");
        }
        
    }
}