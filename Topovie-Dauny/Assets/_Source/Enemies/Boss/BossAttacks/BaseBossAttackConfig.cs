using UnityEngine;

namespace Enemies.Boss.BossAttacks
{
    [CreateAssetMenu(fileName = "Base Attack Config", menuName = "Combat/BossAttacks/Base Attack Config")]
    public class BaseBossAttackConfig: ScriptableObject
    {
        [field:Header("Timing Settings")]
        [field:SerializeField] public float FadeInTime { get; private set; }
        [field:SerializeField] public float WarningDuration { get; private set; }
        [field:SerializeField] public float AttackDuration { get; private set; }
        [field:SerializeField] public float TransitionDuration { get; private set; }
        [field:SerializeField] public float FadeOutTime { get; private set; }
    }
}