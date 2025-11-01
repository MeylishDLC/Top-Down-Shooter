using System;
using Analytics;
using Player.PlayerCombat;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Interactable.HealingStuff
{
    public class HealOrb : MonoBehaviour
    { 
        [field: SerializeField] public int HealAmount { get; private set; } = 10;
        private PlayerHealth _playerHealth;
        private AudioManager _audioManager;
        private AnalyticsManager _analyticsManager;
        
        public void Construct(PlayerHealth playerHealth, AudioManager audioManager, AnalyticsManager analyticsManager)
        {
            _audioManager = audioManager;
            _playerHealth = playerHealth;
            _analyticsManager = analyticsManager;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _playerHealth.Heal(HealAmount);
                _analyticsManager.OnHealOrbCollected();
                DestroyOrb();
            }
        }
        private void DestroyOrb()
        {
            _audioManager.PlayOneShot(_audioManager.FMODEvents.OrbCollectedSound);
            Destroy(gameObject);
        }
    }
}
