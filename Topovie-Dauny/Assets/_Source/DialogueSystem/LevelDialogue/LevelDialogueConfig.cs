using UnityEngine;

namespace DialogueSystem.LevelDialogue
{
    [CreateAssetMenu (fileName = "Level Dialogue Config", menuName = "Level Dialogue/Level Dialogue Config")]
    public class LevelDialogueConfig: ScriptableObject
    {
        [field:SerializeField] public TextAsset DialogueOnStart {get; private set;}
        [field:SerializeField] public TextAsset[] DialoguesAfterCharges {get; private set;}
        [field:SerializeField] public float DelayBeforeStartDialogue {get; private set;}
    }
}