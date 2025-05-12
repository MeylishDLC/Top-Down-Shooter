using System;

namespace Tutorial.Scenarios.ScenariosTypes
{
    public interface ITutorialScenario
    {
        public event Action OnEndScenario;
        public void PerformScenario();
        public void CleanUp();
    }
}