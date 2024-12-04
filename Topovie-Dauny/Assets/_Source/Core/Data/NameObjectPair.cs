using System;
using UnityEngine;

namespace Core.Data
{
    [Serializable]
    public class NameObjectPair<T>
    {
        [field: SerializeField] public string EnemyAssetName { get; private set; }
        [field: SerializeField] public T Object { get; private set; }
    }
}