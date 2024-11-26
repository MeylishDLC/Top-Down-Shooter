using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bullets.Projectile;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.AbilityWeapons
{
    public class BombObject: MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private int timeBeforeBlowMilliseconds;
        [SerializeField] private float blowupDuration;
        [SerializeField] private float blowupRange;
        [SerializeField] private float scaleIncreaseWhenBlowup;
        [SerializeField] private Animator animator;
        [SerializeField] private Projectile projectile;
        
        private static readonly int Blowup = Animator.StringToHash("blowup");
        private void Start()
        {
            projectile.Calculations.OnDestinationReached += BlowUp;
        }
        private void BlowUp()
        {
            projectile.Calculations.OnDestinationReached -= BlowUp;
            BlowUpAsync(CancellationToken.None).Forget();
        }
        private async UniTask BlowUpAsync(CancellationToken token)
        {
            await UniTask.Delay(timeBeforeBlowMilliseconds, cancellationToken: token);

            var colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, blowupRange, colliders);
            
            var filteredColliders = colliders.Where(c => c != null).ToArray();
            
            foreach (var hitCollider in filteredColliders)
            {
                if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (hitCollider.gameObject.TryGetComponent(out IEnemyHealth enemyHealth))
                    {
                        enemyHealth.TakeDamage(damage);
                    }
                }
            }
            
            await gameObject.transform.DOScale(scaleIncreaseWhenBlowup, 0f);
            animator.SetTrigger(Blowup);
            await UniTask.Delay(TimeSpan.FromSeconds(blowupDuration), cancellationToken: token);
            gameObject.SetActive(false);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blowupRange);
        }
    }
}