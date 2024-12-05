using UnityEngine;

namespace UI.UIShop.Dialogue
{
    [CreateAssetMenu (fileName = "Vet Dialogue Config", menuName = "Core/Level Dialogue/Vet Dialogue Config")]
    public class VetDialogueConfig: ScriptableObject
    {
        [field:SerializeField] public DialoguePack[] ChargesDialoguePacks { get; private set; }
    }
}