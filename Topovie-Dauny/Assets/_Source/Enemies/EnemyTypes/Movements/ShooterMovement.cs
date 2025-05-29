using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies.EnemyTypes.Movements;
using FMODUnity;
using Pathfinding;
using SoundSystem;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemies.EnemyTypes
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShooterMovement: EnemyMovement
    { 
        private Shooter _shooter;
        private void Awake()
        {
            _shooter = GetComponent<Shooter>();
            _shooter.OnShootTriggered += DoAttack;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _shooter.OnShootTriggered -= DoAttack;
        }

        protected override void Update()
        {
            if (IsDying)
            {
                return;
            }
            
            if (AIPath.canMove)
            {
               HandleFlipping();
            }
            if (moveSound.IsNull)
            {
                return;
            }
            Timer += Time.deltaTime;
            if (Timer >= soundFrequency)
            {
                AudioManager.PlayOneShot(moveSound, gameObject.transform.position, 
                    PlayerTransform.position, soundDistance);
                Timer = 0;
            }
        }

        private void DoAttack()
        {
            DoAttackAsync(CtOnDestroy).Forget();
        }
        private async UniTask DoAttackAsync(CancellationToken token)
        {
            OnAttackStarted?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(startAttackDuration), cancellationToken: token);
            
            OnAttack?.Invoke();
            _shooter.Shoot();

            await UniTask.Delay(TimeSpan.FromSeconds(remainingAttackDuration), cancellationToken: token);
        }
    }
}