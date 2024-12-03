using Enemies.Pools;
using UnityEngine;

namespace Enemies
{
    public class EnemyContainer: MonoBehaviour
    { 
        [field:SerializeField] public string EnemyPrefabAssetName { get; private set; }
        public EnemyPool Pool { get; private set; }
        public void InjectPool(EnemyPool pool)
        {
            Pool = pool;
        }
    }
}