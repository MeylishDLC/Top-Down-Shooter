using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Player.PlayerCombat;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Aid", menuName = "Combat/Abilities/Aid")]
    public class Aid: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;

        [Header("Specific Settings")] 
        [SerializeField] private int healAmount;

        private PlayerHealth _playerHealth;
        private AudioManager _audioManager;
        public override void Construct(PlayerMovement playerMovement, ProjectContext projectContext)
        {
            _audioManager = projectContext.Container.Resolve<AudioManager>();
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        public override void UseAbility()
        {
            if (Mathf.Approximately(_playerHealth.CurrentHealth, _playerHealth.MaxHealth))
            {
                _audioManager.PlayOneShot(_audioManager.FMODEvents.AidSound);
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