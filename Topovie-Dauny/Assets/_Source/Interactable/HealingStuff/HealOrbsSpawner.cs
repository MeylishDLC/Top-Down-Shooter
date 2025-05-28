using System;
using Enemies;
using Player.PlayerCombat;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Interactable.HealingStuff
{
    public class HealOrbsSpawner
    { 
        private readonly PlayerHealth _playerHealth;
        private readonly int _dropPercentChance;
        private readonly HealOrb _orbPrefab;
        private readonly AudioManager _audioManager;
        
        [Inject]
        public HealOrbsSpawner(HealingOrbsSpawnerConfig config, PlayerMovement playerMovement, AudioManager audioManager)
        {
            if (!config.OrbPrefab)
            {
                throw new Exception("OrbPrefab is null");
            }
            _orbPrefab = config.OrbPrefab;

            _audioManager = audioManager;
            _playerHealth = playerMovement.GetComponent<PlayerHealth>();
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
            var orb = Object.Instantiate(_orbPrefab, spawnPosition, Quaternion.identity);
            orb.Construct(_playerHealth, _audioManager);
            Debug.Log("Spawned orb");
        }
        
    }
}