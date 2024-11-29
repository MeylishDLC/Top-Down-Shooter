using UnityEngine;

namespace Enemies.Boss.Phases
{
    [CreateAssetMenu(fileName = "Phase Config", menuName = "Combat/BossAttacks/Phase Config")]
    public class BasePhaseConfig: ScriptableObject
    {
        [field: SerializeField] public int BossHealth { get; private set; }
        [field: SerializeField] public float MinVulnerabilityDuration { get; private set; }
        [field: SerializeField] public float MaxVulnerabilityDuration { get; private set; }
        [field: SerializeField] public float AttackTransitionDuration { get; private set; }
    }
}