using UnityEngine;
using UnityEngine.Serialization;

namespace Bullets.Projectile
{
    [CreateAssetMenu(fileName = "New Projectile Config", menuName = "Projectile/New Projectile Config")]
    public class ProjectileConfig: ScriptableObject
    {
        [field:SerializeField] public GameObject Prefab { get; private set; }
        [field:SerializeField] public float MaxMoveSpeed { get; private set; }
        [field:SerializeField] public float MaxHeight { get; private set; }
        [field:SerializeField] public AnimationCurve TrajectoryAnimationCurve { get; private set; }
        [field:SerializeField] public AnimationCurve AxisCorrectionAnimationCurve {get; private set;}
        [field: SerializeField] public AnimationCurve ProjectileSpeedAnimationCurve { get; private set; }
    }
}