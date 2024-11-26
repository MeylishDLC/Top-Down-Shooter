using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "CatFood", menuName = "Combat/Abilities/CatFood")]
    public class CatFood: Ability
    {
        [SerializeField] private float newPlayerSpeed;
        [SerializeField] private float boostDuration;

        private float _initSpeed;
        private PlayerMovement _playerMovement;
        
        public override void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            _initSpeed = playerMovement.MovementSpeed;
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            OnAbilitySuccessfullyUsed?.Invoke();
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