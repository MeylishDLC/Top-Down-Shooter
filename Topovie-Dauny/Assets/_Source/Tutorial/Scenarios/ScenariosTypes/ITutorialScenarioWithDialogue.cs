using Core.InputSystem;
using DialogueSystem;

namespace Tutorial.Scenarios.ScenariosTypes
{
    public interface ITutorialScenarioWithDialogue: ITutorialScenario
    {
        public void SetupScenario(InputListener listener, DialogueManager dialogueManager);
    }
}