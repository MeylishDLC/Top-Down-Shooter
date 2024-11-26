using UnityEngine;

namespace Bullets.BulletPatterns
{
    [CreateAssetMenu(fileName = "Bullet Spawner Config", menuName = "Combat/Bullet Spawners/Bullet Spawner Config")]
    public class BulletSpawnerConfig: ScriptableObject
    {
        [field: Header("Bullet Attributes")] 
        [field: SerializeField] public float Lifetime {get; private set;}
        [field: SerializeField] public float Speed {get; private set;}
        [field: SerializeField] public int Damage {get; private set;}

        [field: Header("Spawn Attributes")]
        [field: SerializeField] public SpawnerType SpawnerType {get; private set;}
        [field: SerializeField] public float FiringRate {get; private set;}
        [field: SerializeField] public float SpinSpeed {get; private set;}
    }
}