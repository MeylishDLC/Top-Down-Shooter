using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "CatFood", menuName = "Combat/Abilities/CatFood")]
    public class CatFood: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;
        
        [Header("Specific Settings")] 
        [SerializeField] private float newPlayerSpeed;
        [SerializeField] private float boostDuration;

        private float _initSpeed;
        private PlayerMovement _playerMovement;
        private AudioManager _audioManager;
        public override void Construct(PlayerMovement playerMovement, ProjectContext projectContext)
        {
            _playerMovement = playerMovement;
            _initSpeed = playerMovement.MovementSpeed;
            _audioManager = projectContext.Container.Resolve<AudioManager>();
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            OnAbilitySuccessfullyUsed?.Invoke();
            _audioManager.PlayOneShot(_audioManager.FMODEvents.CatFoodSound);
            ChangeSpeedForTime(token).Forget();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
        private async UniTask ChangeSpeedForTime(CancellationToken token)
        {
            _playerMovement.ChangeSpeed(newPlayerSpeed);
            await UniTask.Delay(TimeSpan.FromSeconds(boostDuration), cancellationToken: token);
            _playerMovement.ChangeSpeed(_initSpeed);
        }
    }
}