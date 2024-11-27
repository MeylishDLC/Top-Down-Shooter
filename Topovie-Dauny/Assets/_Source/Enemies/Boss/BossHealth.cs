using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Boss.BossAttacks;
using Enemies.Boss.BossAttacks.Chessboard;
using Enemies.Boss.BossAttacks.Lasers;
using UnityEngine;

namespace Enemies.Boss
{
    public class BossHealth: MonoBehaviour, IEnemyHealth
    {
        [SerializeField] private ChessboardAttack[] attackers;
        [SerializeField] private LasersAttack[] lasers;
        public void TakeDamage(int damage)
        {
            Test().Forget();
        }

        private async UniTask Test()
        {
            foreach (var laser in lasers)
            {
                await laser.TriggerAttack(CancellationToken.None);
            }

            await UniTask.Delay(1000);

            foreach (var attacker in attackers)
            {
                await attacker.TriggerAttack(CancellationToken.None);
            }
        }
    }
}