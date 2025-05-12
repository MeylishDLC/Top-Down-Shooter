using System;

namespace Tutorial
{
    public interface ITutorialScenario
    {
        public event Action OnEndScenario;
        public void PerformScenario();
        public void CleanUp();
    }
}