using System.Threading;
using Cysharp.Threading.Tasks;
using UI.Tutorial;
using UnityEngine;

namespace Core.Bootstrappers
{
    public class TutorialBootstrapper: BaseLevelBootstrapper
    {
        [SerializeField] private BasicTutorial tutorial;
        protected override UniTask LoadScene(CancellationToken token)
        {
            return base.LoadScene(token).ContinueWith(() => tutorial.EnableTutorial());
        }
    }
}