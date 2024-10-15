using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using Player.PlayerControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Aid", menuName = "Abilities/Aid")]
    public class Aid: Ability
    {
        [Header("Specific Settings")]
        [SerializeField] private int healAmount;

        private PlayerHealth _playerHealth;

        public override void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            _playerHealth.Heal(healAmount);
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
    }
}