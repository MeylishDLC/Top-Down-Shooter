using UnityEngine;
using UnityEngine.UI;

namespace Player.PlayerAbilities
{
    public abstract class Ability: MonoBehaviour
    {
        public bool CanUse { get; protected set; } = true;
        [field:SerializeField] public int CooldownMilliseconds { get; protected set; }
        [field:SerializeField] public Sprite AbilityImage { get; protected set; }
        [field: TextArea]
        [field:SerializeField] public string AbilityDefinition { get; private set; }
        [field: TextArea]
        [field:SerializeField] public string AbilityDescription { get; private set; }
        
        public abstract void UseAbility();
    }
}