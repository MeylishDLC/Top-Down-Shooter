using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Boss.BossAttacks;
using Enemies.Boss.BossAttacks.Chessboard;
using UnityEngine;

namespace Enemies.Boss
{
    public class BossHealth: MonoBehaviour, IEnemyHealth
    {
        [SerializeField] private ChessboardAttack attacker;
        public void TakeDamage(int damage)
        {
            attacker.TriggerAttack(CancellationToken.None).Forget();
        }
    }
}