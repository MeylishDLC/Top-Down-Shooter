using UnityEngine;

namespace Core.EnemyWaveData
{
    [System.Serializable]
    public class EnemySpawnsPair
    {
        [field: SerializeField] public GameObject[] EnemiesPrefabs {get; private set;}
        [field: SerializeField] public Transform[] SpawnPoints {get; private set;}
    }
}