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
        [field: Header("Weapon Sounds")]
        [field: SerializeField] public EventReference ReloadSound { get; private set; }
        
        [field: Header("Dialogue Sounds")]
        [field: SerializeField] public EventReference LeoDialogueSound { get; private set; }
        [field: SerializeField] public EventReference BossDialogueSound { get; private set; }
        [field: SerializeField] public EventReference VetDialogueSound { get; private set; }
    }
}