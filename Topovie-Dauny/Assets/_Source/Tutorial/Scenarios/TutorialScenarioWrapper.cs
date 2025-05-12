namespace Tutorial.Scenarios
{
    [System.Serializable]
    public class TutorialScenarioWrapper
    {
        public string typeName;
        public ITutorialScenario scenario;

        public TutorialScenarioWrapper(ITutorialScenario scenario)
        {
            this.scenario = scenario;
            typeName = scenario.GetType().AssemblyQualifiedName;
        }
    }
}