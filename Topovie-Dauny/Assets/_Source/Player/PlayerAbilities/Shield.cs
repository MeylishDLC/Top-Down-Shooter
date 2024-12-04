using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Shield", menuName = "Combat/Abilities/Shield")]
    public class Shield: Ability
    {
        [SerializeField] private float shieldDuration; 
        [SerializeField] private float shieldBlinkSpeedOnDisappear;
        [SerializeField] private float remainedTimeToStartBlink;
        [SerializeField] private GameObject shieldPrefab;

        private float _initialTransparency;
        private PlayerHealth _playerHealth;
        private SpriteRenderer _shieldRenderer;
        private CancellationTokenSource _invincibilityCts = new();
        
        public override void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.GetComponent<PlayerHealth>();
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            EnableShield(token).Forget();
            OnAbilitySuccessfullyUsed?.Invoke();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
        private async UniTask EnableShield(CancellationToken token)
        {
            _playerHealth.SetCanTakeDamage(false);
            KeepPlayerInvincible(_invincibilityCts.Token).Forget();
            
            var shield = Instantiate(shieldPrefab, _playerHealth.gameObject.transform);
            _shieldRenderer = shield.GetComponent<SpriteRenderer>();
            _initialTransparency = _shieldRenderer.color.a;
            
            //todo: throw exception if negative
            await UniTask.Delay(TimeSpan.FromSeconds(shieldDuration - remainedTimeToStartBlink), cancellationToken: token);

            var loopsAmount = (int)Math.Round(remainedTimeToStartBlink / shieldBlinkSpeedOnDisappear);
            await _shieldRenderer.DOFade(0f, shieldBlinkSpeedOnDisappear).SetLoops(loopsAmount, LoopType.Yoyo);
            
            CancelInvincibility();
            _playerHealth.SetCanTakeDamage(true);
            _shieldRenderer.gameObject.SetActive(false);
            
            await _shieldRenderer.DOFade(_initialTransparency, 0f);
            Destroy(shield);
        }
        //todo refactor
        private async UniTask KeepPlayerInvincible(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _playerHealth.SetCanTakeDamage(false);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        private void CancelInvincibility()
        {
            _invincibilityCts.Cancel();
            _invincibilityCts.Dispose();
            _invincibilityCts = new CancellationTokenSource();
        }
    }
}