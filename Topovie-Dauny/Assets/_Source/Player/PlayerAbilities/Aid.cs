using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Aid: Ability
    {
        [Header("Specific Settings")]
        [SerializeField] private int healAmount;

        private PlayerHealth _playerHealth;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        public override void UseAbility()
        {
            if (CanUse)
            {
                UseAbilityAsync().Forget();
            }
        }
        private async UniTask UseAbilityAsync()
        {
            CanUse = false;
            _playerHealth.Heal(healAmount);
            await UniTask.Delay(CooldownMilliseconds);
            CanUse = true;
        }
    }
}