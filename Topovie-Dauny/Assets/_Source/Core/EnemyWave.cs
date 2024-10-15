using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class EnemyWave
    {
        [field:SerializeField] public Transform[] SpawnPoints { get; private set; }
        [field:SerializeField] public GameObject[] EnemyPrefabs { get; private set; }
        [field:SerializeField] public int MaxEnemySpawnAtOnce { get; private set; }
        [field:SerializeField] public int MinEnemySpawnAtOnce { get; private set; }
        
        [field:Header("Time Settings")]
        [field:SerializeField] public int MaxTimeBetweenSpawnMilliseconds { get; private set; }
        [field:SerializeField] public int MinTimeBetweenSpawnMilliseconds { get; private set; }
        [field:SerializeField] public float TimeToActivatePencil { get; private set; }
    }
}