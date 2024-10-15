using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player.PlayerCombat;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Shield: Ability
    {
        [SerializeField] private float shieldDuration;
        [SerializeField] private SpriteRenderer shield;
        [SerializeField] private float shieldBlinkSpeedOnDisappear;
        [SerializeField] private float remainedTimeToStartBlink;

        private float _initialTransparency;
        private PlayerHealth _playerHealth;
        
        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.GetComponent<PlayerHealth>();
            _initialTransparency = shield.color.a;
        }
        public override void UseAbility()
        {
            if (CanUse)
            {
                UseAbilityAsync(CancellationToken.None).Forget();
            }
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            CanUse = false;
            EnableShield(token).Forget();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
            CanUse = true;
        }
        private async UniTask EnableShield(CancellationToken token)
        {
            //todo: visualize soon disappearance of shield
            _playerHealth.SetCanTakeDamage(false);
            shield.gameObject.SetActive(true);
            
            //todo: throw exception if negative
            await UniTask.Delay(TimeSpan.FromSeconds(shieldDuration - remainedTimeToStartBlink), cancellationToken: token);

            var loopsAmount = (int)Math.Round(remainedTimeToStartBlink / shieldBlinkSpeedOnDisappear);
            await shield.DOFade(0f, shieldBlinkSpeedOnDisappear).SetLoops(loopsAmount, LoopType.Yoyo);
            
            _playerHealth.SetCanTakeDamage(true);
            shield.gameObject.SetActive(false);
            await shield.DOFade(_initialTransparency, 0f);
        }
    }
}