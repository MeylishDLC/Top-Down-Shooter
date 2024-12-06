using System;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    public abstract class Ability: ScriptableObject
    {
        //todo use ability and start cooldown ONLY on successfully used
        public abstract event Action OnAbilitySuccessfullyUsed;
        [field:SerializeField] public int CooldownMilliseconds { get; protected set; }
        [field:SerializeField] public Sprite AbilityImage { get; protected set; }
        [field: TextArea]
        [field:SerializeField] public string AbilityDefinition { get; private set; }
        [field: TextArea]
        [field:SerializeField] public string AbilityDescription { get; private set; }
        [field:SerializeField] public AbilityTypes AbilityType { get; protected set; } = AbilityTypes.TapButton;
        public abstract void Construct(PlayerMovement playerMovement, ProjectContext projectContext);
        public abstract void UseAbility();
    }

    public enum AbilityTypes
    {
        HoldButton,
        TapButton
    }
}