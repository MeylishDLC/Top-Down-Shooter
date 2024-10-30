using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        
        private static readonly int Blowup = Animator.StringToHash("blowup");
        private Collider2D _collider;
        private void Start()
        {
            _collider = GetComponent<Collider2D>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            BlowUp(CancellationToken.None).Forget();
        }
        private async UniTask BlowUp(CancellationToken token)
        {
            _collider.enabled = false;
            await UniTask.Delay(timeBeforeBlowMilliseconds, cancellationToken: token);

            var colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, blowupRange, colliders);
            
            var filteredColliders = colliders.Where(c => c != null).ToArray();
            
            foreach (var hitCollider in filteredColliders)
            {
                if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (hitCollider.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
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