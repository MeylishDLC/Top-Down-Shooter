using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.LoadingSystem
{
    public class AssetLoader
    {
        private GameObject _instance;

        public async UniTask<GameObject> LoadGameObject(AssetReferenceGameObject reference, CancellationToken token)
        {
            if (reference.RuntimeKeyIsValid())
            {
                try
                {
                    await Addressables.LoadAssetAsync<GameObject>(reference).ToUniTask(cancellationToken: token)
                        .ContinueWith((obj) => _instance = obj);
                }
                catch
                {
                    Debug.LogWarning("Couldn't load the asset");
                }
            }
            else
            { 
                throw new Exception("Reference is not valid");
            }
            return _instance;
        }
        public void ReleaseStoredInstance()
        {
            Addressables.Release(_instance);
        }
    }
}