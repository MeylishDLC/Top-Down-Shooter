using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Bullets.Projectile;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies;
using UnityEngine;

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
            DealDamageInRange();
            
            await gameObject.transform.DOScale(scaleIncreaseWhenBlowup, 0f);
            animator.SetTrigger(Blowup);
            await UniTask.Delay(TimeSpan.FromSeconds(blowupDuration), cancellationToken: token);
            gameObject.SetActive(false);
        }
        private void DealDamageInRange()
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, blowupRange);

            foreach (var col in hitColliders)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Enemy") &&
                    col.gameObject.transform.parent.TryGetComponent<IEnemyHealth>(out var enemyHealth))
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blowupRange);
        }
    }
}