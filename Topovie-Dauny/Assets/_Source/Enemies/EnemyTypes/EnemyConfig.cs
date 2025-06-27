using FMODUnity;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    [CreateAssetMenu(fileName = "General Enemy Config", menuName = "Combat/Enemies/General Enemy Config")]

    public class EnemyConfig: ScriptableObject
    {
        [field: Header("Sound")]
        [field: SerializeField] public EventReference MoveSound {get; private set;}
        [field: SerializeField] public float SoundFrequency { get; private set; } = 1f;
        [field: SerializeField] public float SoundDistance {get; private set;} = 2f;

        [field: Header("Attack")] 
        [field: SerializeField] public float AttackRange {get; private set;} = 1.5f;
        [field: SerializeField] public float StartAttackDuration {get; private set;} = 1.5f;
        [field: SerializeField] public float RemainingAttackDuration {get; private set;} = 1.5f;
        [field: SerializeField] public float DeathDuration {get; private set;} = 1.5f;
    }
}