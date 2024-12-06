using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace SoundSystem
{
    [CreateAssetMenu(fileName = "FMOD Events Config", menuName = "Core/Audio/FMOD Events Config")]
    public class FMODEvents: ScriptableObject
    {
        [field: Header("----MUSIC----")] 
        [field: SerializeField] public EventReference MenuMusic {get; private set;}
        [field: SerializeField] public List<EventReference> LevelsMusic {get; private set;}
        [field: SerializeField] public EventReference BossFightMusic { get; private set; }
        
        [field: Header("\n----SFX----")]
        [field: SerializeField] public EventReference ReloadSound { get; private set; }

    }
}