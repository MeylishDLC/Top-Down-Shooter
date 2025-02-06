using UnityEngine;

namespace Interactable.HealingStuff
{
    [CreateAssetMenu (menuName = "Interactable/Orbs Spawner Config")]
    public class HealingOrbsSpawnerConfig: ScriptableObject
    {
        [field: Range(0,100)]
        [field: SerializeField] public int SpawnChancePercent { get; private set; }
        [field: SerializeField] public HealOrb OrbPrefab { get; private set; }
    }
}