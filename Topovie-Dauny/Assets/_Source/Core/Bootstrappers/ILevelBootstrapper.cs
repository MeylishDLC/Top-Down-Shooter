using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Bootstrappers
{
    public interface ILevelBootstrapper
    {
        public UniTask InstantiateAssets(CancellationToken token);
        public void InitializePools();
        public void InitializeGuns();
        public void CleanUpPools();
    }
}