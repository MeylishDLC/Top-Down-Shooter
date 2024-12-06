using FMODUnity;
using UnityEngine;

namespace SoundSystem.DialogueSoundSO
{
    [CreateAssetMenu (fileName = "Dialogue Audio Info", menuName = "Core/Audio/Dialogue Audio Info")]
    public class DialogueAudioInfoSO: ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }

        [field: SerializeField] public EventReference[] TypeSounds { get; private set; }
        [field:Range(1f, 5f)]
        [field: SerializeField]public int FrequencyLvl { get; private set; } = 2; 
    }
}