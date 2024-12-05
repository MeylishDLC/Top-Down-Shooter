using System.Collections.Generic;
using Player.PlayerAbilities;
using UnityEngine;

namespace UI.UIShop.Dialogue
{
    [System.Serializable]
    public class DialoguePack
    {
        [field:TextArea]
        [field: SerializeField] public string Greeting { get; private set; }
        [field:TextArea]
        [field: SerializeField] public string Goodbye { get; private set; }
        [field: SerializeField] public List<AbilityTypeDialoguePair> AbilityDialoguePairs { get; private set; }
        
        [System.Serializable]
        public class AbilityTypeDialoguePair
        {
            [field:TypeConstraint(typeof(Ability), AllowAbstract = false, AllowObsolete = false, 
                TypeSettings = TypeSettings.Class, TypeGrouping = TypeGrouping.None)]
            [field: SerializeField] public SerializedType AbilityType { get; private set; }
            [field:TextArea]
            [field: SerializeField] public string Dialogue { get; private set; }
        }
    }
}