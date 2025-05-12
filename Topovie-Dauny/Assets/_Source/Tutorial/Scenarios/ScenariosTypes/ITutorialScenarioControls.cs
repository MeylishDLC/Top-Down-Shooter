using Core.InputSystem;

namespace Tutorial.Scenarios.ScenariosTypes
{
    public interface ITutorialScenarioControls: ITutorialScenario
    {
        public void SetupScenario(InputListener listener);
    }
}