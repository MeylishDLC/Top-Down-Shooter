using Core.InputSystem;

namespace Tutorial
{
    public interface ITutorialScenarioControls: ITutorialScenario
    {
        public void SetupScenario(InputListener listener);
    }
}