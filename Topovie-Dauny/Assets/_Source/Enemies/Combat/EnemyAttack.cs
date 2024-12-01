using System;
using Player.PlayerCombat;
using UnityEngine;
using Zenject;

namespace Enemies.Combat
{
    public class EnemyAttack: MonoBehaviour
    {
        [field:SerializeField] public int Attack { get; private set; }

        private PlayerHealth _playerHealth;
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (_playerHealth == null)
                {
                    _playerHealth = other.GetComponent<PlayerHealth>();
                }
                _playerHealth.TakeDamageWithKnockback(Attack, transform);
            }
        }
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (_playerHealth == null)
                {
                    _playerHealth = other.GetComponent<PlayerHealth>();
                }
                _playerHealth.TakeDamageWithKnockback(Attack, transform);
            }
        }
    }
}