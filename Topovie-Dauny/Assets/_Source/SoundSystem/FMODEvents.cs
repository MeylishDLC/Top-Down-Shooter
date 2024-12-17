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
        [field: SerializeField] public EventReference EndingMusic { get; private set; }
        [field: SerializeField] public EventReference DeathMusic { get; private set; }
        
        [field: Header("\n----SFX----")]
        [field: SerializeField] public EventReference ReloadSound { get; private set; }
        [field: SerializeField] public EventReference BugBlowUpSound { get; private set; }
        [field: SerializeField] public EventReference RpgBlowUpSound { get; private set; }
        
        [field: Header("Abilities Sounds")]
        [field: SerializeField] public EventReference AidSound { get; private set; }
        [field: SerializeField] public EventReference BombSound { get; private set; }
        [field: SerializeField] public EventReference KnifeSound { get; private set; }
        [field: SerializeField] public EventReference ShieldSound { get; private set; }
        [field: SerializeField] public EventReference CatFoodSound { get; private set; }
        [field: SerializeField] public EventReference StunSound { get; private set; }
        
        [field: Header("UI Sounds")]
        [field: SerializeField] public EventReference ShopButtonSound { get; private set; }
        [field: SerializeField] public EventReference ShopEnterSound { get; private set; }
        [field: SerializeField] public EventReference UIButtonSound { get; private set; }
        
        [field:Header("Level Sounds")]
        [field: SerializeField] public EventReference ChargingSound { get; private set; }
        [field: SerializeField] public EventReference AttackStartedSound { get; private set; }
        [field: SerializeField] public EventReference PortalEnabledSound { get; private set; }
        
        [field: Header("Boss Sounds")]
        [field: SerializeField] public EventReference LeoTransformationSound { get; private set; }
        [field: SerializeField] public EventReference ChessboardSound { get; private set; }
        [field: SerializeField] public EventReference LasersSound { get; private set; }
        [field: SerializeField] public EventReference FireballSound { get; private set; }
        [field: SerializeField] public EventReference LinesSound { get; private set; }
    }
}