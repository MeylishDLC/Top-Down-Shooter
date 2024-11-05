using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.EnemyWaveData
{
    [System.Serializable]
    public class EnemySpawnsPair
    {
        public IReadOnlyList<GameObject> EnemiesPrefabs => _enemiesPrefabs;
        private List<GameObject> _enemiesPrefabs = new();
        [field: SerializeField] public AssetReferenceGameObject[] EnemiesAssets {get; private set;}
        [field: SerializeField] public Transform[] SpawnPoints {get; private set;}
        public async UniTask LoadAssets(CancellationToken token)
        {
            _enemiesPrefabs.Clear();

            foreach (var enemyAsset in EnemiesAssets)
            {
                if (enemyAsset.RuntimeKeyIsValid())
                {
                    var handle = enemyAsset.Asset as GameObject;

                    if (handle == null) 
                    {
                        var loadedAsset = await enemyAsset.LoadAssetAsync().ToUniTask(cancellationToken: token);
                        if (loadedAsset != null)
                        {
                            _enemiesPrefabs.Add(loadedAsset);
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to load asset for {enemyAsset}");
                        }
                    }
                    else
                    {
                        _enemiesPrefabs.Add(handle); 
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid asset reference in EnemiesAssets array at index " +
                                     $"{System.Array.IndexOf(EnemiesAssets, enemyAsset)}");
                }
            }
        }

        public void UnloadAssets()
        {
            foreach (var enemyAsset in EnemiesAssets)
            {
                if (enemyAsset.RuntimeKeyIsValid())
                {
                    if (enemyAsset.Asset != null)
                    {
                        enemyAsset.ReleaseAsset();
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid asset reference in EnemiesAssets array at index " +
                                     $"{System.Array.IndexOf(EnemiesAssets, enemyAsset)}");
                }
            }
            _enemiesPrefabs.Clear();
        }
    }
}