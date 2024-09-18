using System;
using Player.PlayerCombat;
using Player.PlayerMovement;
using UnityEngine;
using Zenject;

namespace Enemies.Combat
{
    public class EnemyAttack: MonoBehaviour
    {
        [field:SerializeField] public int Attack { get; private set; }

        private PlayerHealth _playerHealth;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (_playerHealth == null)
                {
                    _playerHealth = other.GetComponent<PlayerHealth>();
                }
                
                _playerHealth.TakeDamage(Attack);
                _playerHealth.KnockBack.GetKnockedBack(transform);
                
                if (gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (_playerHealth == null)
                {
                    _playerHealth = other.GetComponent<PlayerHealth>();
                }
                
                _playerHealth.TakeDamage(Attack);
                _playerHealth.KnockBack.GetKnockedBack(transform);

                if (gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}