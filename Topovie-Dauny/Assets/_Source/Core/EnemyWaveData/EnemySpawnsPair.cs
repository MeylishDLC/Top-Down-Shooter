using System.Collections.Generic;
using System.Threading;
using Core.LoadingSystem;
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
        
        private List<AssetLoader> loaders = new();
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
                        var loader = new AssetLoader();
                        //var loadedAsset = await enemyAsset.LoadAssetAsync().ToUniTask(cancellationToken: token);
                        
                        var loadedAsset = await loader.LoadGameObject(enemyAsset, token);
                        if (loadedAsset != null)
                        {
                            _enemiesPrefabs.Add(loadedAsset);
                            loaders.Add(loader);
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
            /*foreach (var enemyAsset in EnemiesAssets)
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
            }*/

            foreach (var loader in loaders)
            {
                loader.ReleaseStoredInstance();
            }
            _enemiesPrefabs.Clear();
        }
    }
}