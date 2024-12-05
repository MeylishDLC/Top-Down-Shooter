using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Aid", menuName = "Combat/Abilities/Aid")]
    public class Aid: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;

        [Header("Specific Settings")]
        [SerializeField] private int healAmount;

        private PlayerHealth _playerHealth;
        public override void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        public override void UseAbility()
        {
            if (Mathf.Approximately(_playerHealth.CurrentHealth, _playerHealth.MaxHealth))
            {
                return;
            }
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            _playerHealth.Heal(healAmount);
            OnAbilitySuccessfullyUsed?.Invoke();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
    }
}