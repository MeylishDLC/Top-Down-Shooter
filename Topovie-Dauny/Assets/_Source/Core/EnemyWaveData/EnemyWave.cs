using UnityEngine;

namespace Core.EnemyWaveData
{
    [System.Serializable]
    public class EnemyWave
    {
        [field:SerializeField] public EnemySpawnsPair[] EnemySpawnsPairs { get; private set; }
        [field:SerializeField] public int MaxEnemySpawnAtOnce { get; private set; }
        [field:SerializeField] public int MinEnemySpawnAtOnce { get; private set; }
        
        [field:Header("Time Settings")]
        [field:SerializeField] public float MaxTimeBetweenSpawn { get; private set; }
        [field:SerializeField] public float MinTimeBetweenSpawn { get; private set; }
        [field:SerializeField] public float TimeToActivatePencil { get; private set; }
    }
}