using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    public class CatFood: Ability
    {
        [SerializeField] private float newPlayerSpeed;
        [SerializeField] private float boostDuration;

        private float _initSpeed;
        private PlayerMovement.PlayerMovement _playerMovement;
        
        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            _initSpeed = playerMovement.MovementSpeed;
        }
        public override void UseAbility()
        {
            if (CanUse)
            {
                UseAbilityAsync(CancellationToken.None).Forget( );
            }
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            CanUse = false;
            ChangeSpeedForTime(token).Forget();
            await UniTask.Delay(CooldownMilliseconds);
            CanUse = true;
        }
        private async UniTask ChangeSpeedForTime(CancellationToken token)
        {
            _playerMovement.ChangeSpeed(newPlayerSpeed);
            await UniTask.Delay(TimeSpan.FromSeconds(boostDuration));
            _playerMovement.ChangeSpeed(_initSpeed);
        }
    }
}