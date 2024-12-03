using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Boss.BossAttacks.AttackPairs
{
    public class SequentialAttacks: MonoBehaviour, IBossAttack
    {
        [BossAttack]
        [SerializeField] private List<MonoBehaviour> attacks;
        [SerializeField] private float delayBetweenAttacks = 0.5f;
        [SerializeField] private bool randomiseAttacksDirection = true;
        
        private List<IBossAttack> _bossAttacks;
        private bool _isAttackingOpposite;
        private void Awake()
        {
            _bossAttacks = GetCastedBossAttacksList();
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            return TriggerAllAttacksAsync(token);
        }
        private async UniTask TriggerAllAttacksAsync(CancellationToken token)
        {
            if (randomiseAttacksDirection)
            {
                SetRandomAttackDirection();
                var start = _isAttackingOpposite ? attacks.Count - 1 : 0;
                var end = _isAttackingOpposite ? -1 : attacks.Count;
                var step = _isAttackingOpposite ? -1 : 1;

                for (int i = start; i != end; i += step)
                {
                    await _bossAttacks[i].TriggerAttack(token);
                    await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAttacks), cancellationToken: token);
                }
            }
            else
            {
                for (int i = 0; i < attacks.Count; i++)
                {
                    await _bossAttacks[i].TriggerAttack(token);
                    await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAttacks), cancellationToken: token);
                }
            }
            
        }
        private List<IBossAttack> GetCastedBossAttacksList()
        {
            return attacks.Select(x => x.GetComponent<IBossAttack>()).ToList();
        }
        private void SetRandomAttackDirection()
        {
            _isAttackingOpposite = Random.Range(0, 2) == 0;
        }
    }
}