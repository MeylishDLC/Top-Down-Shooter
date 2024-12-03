using Enemies;
using UnityEngine;

namespace Core.Data
{
    [System.Serializable]
    public class EnemySpawnsPair
    {
        //todo here pools of required enemies

        [field:SerializeField] public EnemyContainer[] Enemies {get; private set;}
        [field: SerializeField] public Transform[] SpawnPoints {get; private set;}
    }
}