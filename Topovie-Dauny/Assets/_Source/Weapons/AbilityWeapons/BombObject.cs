using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Bullets.Projectile;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies;
using FMODUnity;
using SoundSystem;
using UnityEngine;
using Zenject;

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
        private AudioManager _audioManager;
        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
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
            await DisplayBlowUp(token);
            await UniTask.Delay(TimeSpan.FromSeconds(blowupDuration), cancellationToken: token);
            
            gameObject.SetActive(false);
        }
        private UniTask DisplayBlowUp(CancellationToken token)
        {
            return gameObject.transform.DOScale(scaleIncreaseWhenBlowup, 0f).ToUniTask(cancellationToken: token)
                .ContinueWith(() => animator.SetTrigger(Blowup))
                .ContinueWith(() => _audioManager.PlayOneShot(_audioManager.FMODEvents.BombSound));
        }
        private void DealDamageInRange()
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, blowupRange);
            var filteredColliders = hitColliders.Where(c => c != null).ToArray();
            
            foreach (var col in filteredColliders)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (col.gameObject.transform.parent.TryGetComponent<IEnemyHealth>(out var enemyHealth))
                    {
                        enemyHealth.TakeDamage(damage);
                    }
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