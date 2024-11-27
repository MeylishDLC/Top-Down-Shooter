using System.Threading;
using Cysharp.Threading.Tasks;

namespace Enemies.Boss.BossAttacks
{
    public interface IBossAttack
    {
        public UniTask TriggerAttack(CancellationToken token);
    }
}