using System;
using Core.PoolingSystem;
using Player.PlayerCombat;
using UnityEngine;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyBullet: Bullet
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                CancelDisableRecreateCts();
                gameObject.SetActive(false);
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                var playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
                
                playerHealth.KnockBack.GetKnockedBack(gameObject.transform);
                playerHealth.TakeDamage(damageAmount);
                
                CancelDisableRecreateCts();
                gameObject.SetActive(false);
            }
        }
    }
}