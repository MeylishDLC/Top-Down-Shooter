using Player.PlayerCombat;
using UnityEngine;

namespace Enemies.Combat
{
    public class EnemyAttack: MonoBehaviour
    {
        [field:SerializeField] public int Attack { get; private set; }
        protected PlayerHealth PlayerHealth;
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (PlayerHealth == null)
                {
                    PlayerHealth = other.GetComponent<PlayerHealth>();
                }
                PlayerHealth.TakeDamageWithKnockback(Attack, transform);
            }
        }
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (PlayerHealth == null)
                {
                    PlayerHealth = other.GetComponent<PlayerHealth>();
                }
                PlayerHealth.TakeDamageWithKnockback(Attack, transform);
            }
        }
    }
}