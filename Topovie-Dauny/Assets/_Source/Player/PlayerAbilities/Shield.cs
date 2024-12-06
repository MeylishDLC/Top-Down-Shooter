using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Player.PlayerCombat;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Shield", menuName = "Combat/Abilities/Shield")]
    public class Shield: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;

        [Header("Specific Settings")] 
        [SerializeField] private float shieldDuration; 
        [SerializeField] private float shieldBlinkSpeedOnDisappear;
        [SerializeField] private float blinkDuration = 2;
        [SerializeField] private GameObject shieldPrefab;
        [SerializeField] private EventReference useSound;

        private float _initialTransparency;
        private PlayerHealth _playerHealth;
        private AudioManager _audioManager;
        private SpriteRenderer _shieldRenderer;
        private CancellationTokenSource _invincibilityCts = new();
        public override void Construct(PlayerMovement playerMovement, ProjectContext projectContext)
        {
            _audioManager = projectContext.Container.Resolve<AudioManager>();
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
            InstantiateShield();
            _audioManager.PlayOneShot(useSound);
            await UniTask.Delay(TimeSpan.FromSeconds(shieldDuration - blinkDuration), cancellationToken: token);
            await Blink(token);
            await DisableShield(token);
        }
        private void InstantiateShield()
        {
            var shield = Instantiate(shieldPrefab, _playerHealth.gameObject.transform);
            _shieldRenderer = shield.GetComponent<SpriteRenderer>();
            _initialTransparency = _shieldRenderer.color.a;
        }
        private UniTask Blink(CancellationToken token)
        {
            if (shieldDuration - blinkDuration <= 0)
            {
                throw new Exception("Blink duration can't be bigger than shield duration");
            }
            var loopsAmount = (int)Math.Round(blinkDuration / shieldBlinkSpeedOnDisappear);
            return _shieldRenderer.DOFade(0f, shieldBlinkSpeedOnDisappear).SetLoops(loopsAmount, LoopType.Yoyo)
                .ToUniTask(cancellationToken: token);
        }
        private UniTask DisableShield(CancellationToken token)
        {
            CancelInvincibility();
            _playerHealth.SetCanTakeDamage(true);
            _shieldRenderer.gameObject.SetActive(false);
            
            return _shieldRenderer.DOFade(_initialTransparency, 0f).ToUniTask(cancellationToken: token)
                .ContinueWith(() => Destroy(_shieldRenderer.gameObject));
        }
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