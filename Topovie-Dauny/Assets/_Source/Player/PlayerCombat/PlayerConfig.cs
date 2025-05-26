using UnityEngine;

namespace Player.PlayerCombat
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Player/Player Config")]
    public class PlayerConfig: ScriptableObject
    {
        [field:Header("Player Health Settings")]
        [field:SerializeField] public float MaxHealth { get; private set; } = 100;
        
        [field:Header("Knockback Settings")] 
        [field:SerializeField] public float KnockbackThrust { get; private set; } = 1.1f;
        [field:SerializeField] public float KnockbackTime{ get; private set; } = 0.2f;
        [field:SerializeField] public float InvincibilityTime { get; private set; } = 0.5f;
        
        [field: Header("Player Movement Settings")]
        [field:SerializeField] public float MovementSpeed { get; private set; } = 1.5f;
        [field:SerializeField] public float DodgeSpeed { get; private set; } = 20f;
        [field:SerializeField] public float DodgeTime { get; private set; } = 0.5f;
        
        [field:Header("Player Damaged Displaying")]
        [field:SerializeField] public float VignetteDisplayDuration { get; private set; } = 1.0f;
        [field:SerializeField] public float DamagedLightDisplayDuration { get; private set; } = 0.3f;
        
        [field:Header("Player Death")]
        [field:SerializeField] public float PlayerDeathAnimationDuration { get; private set; } = 0.6f;


    }
}