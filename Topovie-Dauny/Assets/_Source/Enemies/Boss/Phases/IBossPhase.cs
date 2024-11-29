using System;

namespace Enemies.Boss.Phases
{
    public interface IBossPhase
    {
        public event Action<PhaseState> OnPhaseStateChanged;
        public BasePhaseConfig PhaseConfig { get; }
        public void StartPhase();
        public void FinishPhase();
    }
}