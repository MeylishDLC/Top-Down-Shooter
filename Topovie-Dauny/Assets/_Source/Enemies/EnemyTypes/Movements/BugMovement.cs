using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.EnemyTypes.Bug;
using UnityEngine;

namespace Enemies.EnemyTypes.Movements
{
    public class BugMovement: EnemyMovement
    {
        [SerializeField] private AreaAttacker areaAttacker;

        private void Awake()
        {
            areaAttacker.OnWarnStarted += DoAttack;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            areaAttacker.OnWarnStarted -= DoAttack;
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

            await UniTask.Delay(TimeSpan.FromSeconds(remainingAttackDuration), cancellationToken: token);
        }
    }
}