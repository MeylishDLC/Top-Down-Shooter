using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.BossAttacks.AttackPairs
{
    public class SimultaneousAttacks: MonoBehaviour, IBossAttack
    {
        [BossAttack]
        [SerializeField] List<MonoBehaviour> attacks;
        
        private List<IBossAttack> _bossAttacks;
        private CancellationToken _destroyCancellationToken;

        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _bossAttacks = GetCastedBossAttacksList();
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            return TriggerAllAttacks(_destroyCancellationToken);
        }
        private UniTask TriggerAllAttacks(CancellationToken token)
        {
            var tasks = Enumerable.Select(_bossAttacks, attack => attack.TriggerAttack(token)).ToList();
            return UniTask.WhenAll(tasks);
        }
        private List<IBossAttack> GetCastedBossAttacksList()
        {
            return attacks.Select(x => x.GetComponent<IBossAttack>()).ToList();
        }
    }
}