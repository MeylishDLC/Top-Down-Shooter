using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies;
using Player.PlayerCombat;
using UnityEngine;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyBullet: BaseBullet, IPoolObject<EnemyBullet>
    {
        public event Action<EnemyBullet> OnObjectDisabled;
        private void OnDisable()
        {
            OnObjectDisabled?.Invoke(this);
            CancelRecreateCts();
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(damageAmount);
                
                CancelRecreateCts();
                gameObject.SetActive(false);
            }
        }
    }
}