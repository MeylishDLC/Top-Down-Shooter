using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class EnemyWave
    {
        [field:SerializeField] public Transform[] SpawnPoints { get; private set; }
        [field:SerializeField] public float SpawnRange { get; private set; }
        [field:SerializeField] public GameObject[] EnemiesPrefabs { get; private set; }
        [field:SerializeField] public int MaxEnemySpawnAtOnce { get; private set; }
        [field:SerializeField] public bool randomizeEnemySpawnAmount { get; private set; }
        
        [field:Header("Time Settings")]
        [field:SerializeField] public int TimeBetweenSpawnMilliseconds { get; private set; }
        [field:SerializeField] public float WaveDurationSeconds { get; private set; }
    }
}