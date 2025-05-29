using Enemies.EnemyTypes;
using Player.PlayerCombat;
using UnityEngine;

namespace Enemies.Combat
{
    public class AnimatedEnemyAttack: EnemyAttack
    {
        [SerializeField] private EnemyMovement enemyMovement;

        private bool _playerInRange;
        private void Awake()
        {
            enemyMovement.OnAttack += PerformAttack;
        }
        private void OnDestroy()
        {
            enemyMovement.OnAttack -= PerformAttack;
        }
        protected override void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }
            PlayerHealth = other.GetComponent<PlayerHealth>();
            if (PlayerHealth)
            {
                _playerInRange = true;
            }
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }
            PlayerHealth = other.GetComponent<PlayerHealth>();
            if (PlayerHealth)
            {
                _playerInRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }
            if (other.GetComponent<PlayerHealth>() == PlayerHealth)
            {
                _playerInRange = false;
                PlayerHealth = null;
            }
        }

        private void PerformAttack()
        {
            if (!_playerInRange || !PlayerHealth)
            {
                return;
            }
            PlayerHealth.TakeDamageWithKnockback(Attack, transform);
        }
    }
}