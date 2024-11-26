using System;
using System.Threading;
using Core.PoolingSystem;
using Cysharp.Threading.Tasks;
using Enemies;
using UnityEngine;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : BaseBullet, IPoolObject<Bullet>
    {
        public event Action<Bullet> OnObjectDisabled;
        private void OnDisable()
        {
            OnObjectDisabled?.Invoke(this);
            CancelRecreateCts();
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemyHealth = other.gameObject.GetComponentInParent<IEnemyHealth>();
                enemyHealth.TakeDamage(damageAmount);
                
                CancelRecreateCts();
                gameObject.SetActive(false);
            }
        }
    }
}
