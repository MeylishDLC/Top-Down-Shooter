using System.Collections.Generic;
using UnityEngine;

namespace UI.Core.Loading
{
    [CreateAssetMenu (fileName = "Loading Tips Config", menuName = "Core/UI/Loading Tips Config")]
    public class LoadingTipsConfig: ScriptableObject
    {
        [field: TextArea]
        [field: SerializeField] public List<string> Tips { get; private set; }
    }
}