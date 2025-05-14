using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player.PlayerControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerDamagedDisplay
    {
        private static readonly int FlashAmountProperty = Shader.PropertyToID("_FlashAmount");
        private readonly float _vignetteDisplayDuration;
        private readonly float _damagedLightDisplayDuration;
        
        private float _initVignetteIntensity;
        private Vignette _vignette;
        private readonly Material _playerDamagedMaterial;
        
        private Tween _currentTween;
        private CancellationTokenSource _damageDisplayCts;
        
        private readonly PlayerHealth _playerHealth;

        [Inject]
        public PlayerDamagedDisplay(PlayerConfig config, Volume vignetteVolume,
            Material playerDamagedMaterial, PlayerMovement playerMovement)
        {
            _vignetteDisplayDuration = config.VignetteDisplayDuration;
            _damagedLightDisplayDuration = config.DamagedLightDisplayDuration;
            
            _playerDamagedMaterial = playerDamagedMaterial;
            
            _damageDisplayCts = new CancellationTokenSource();
            
            SetFlashAmount(0);
            InitializeVignette(vignetteVolume);
            
            _playerHealth = playerMovement.GetComponent<PlayerHealth>();
            _playerHealth.OnDamageTaken += ShowPlayerDamaged;
        }
        public void CleanUp()
        {
            _playerHealth.OnDamageTaken -= ShowPlayerDamaged;
            _damageDisplayCts?.Cancel();
            _damageDisplayCts?.Dispose();
        }
        private void ShowPlayerDamaged(float _)
        {
            CancelRecreateCts();
            ShowPlayerDamagedAsync(_damageDisplayCts.Token).Forget();
        }
        private async UniTask ShowPlayerDamagedAsync(CancellationToken token)
        {
            try
            {
                ShowVignette();
                await ShowPlayerDamagedLightAsync(token);
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
        private void InitializeVignette(Volume vignetteVolume)
        {
            if (vignetteVolume.profile.TryGet<Vignette>(out var vignette))
            {
                _vignette = vignette;
                _initVignetteIntensity = vignette.intensity.value;
                SetVignetteIntensity(0f);
            }
            else
            {
                throw new Exception("Vignette profile doesn't contain Vignette");
            }
        }
        private void SetVignetteIntensity(float value) => _vignette.intensity.value = value;
        private float GetVignetteIntensity() => _vignette.intensity.value;

        private void SetFlashAmount(float value) => _playerDamagedMaterial.SetFloat(FlashAmountProperty, value);
        private async UniTask ShowPlayerDamagedLightAsync(CancellationToken token)
        {
            try
            {
                SetFlashAmount(1);
                await UniTask.Delay(TimeSpan.FromSeconds(_damagedLightDisplayDuration), cancellationToken: token);
                SetFlashAmount(0);
            }
            catch (OperationCanceledException)
            {
                //
            }
        }

        private void ShowVignette()
        {
            KillTween();
            _currentTween = DOTween.To(
                GetVignetteIntensity,
                SetVignetteIntensity,
                _initVignetteIntensity,
                _vignetteDisplayDuration
            ).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        private void KillTween()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
                SetVignetteIntensity(0);
            }
        }
        private void CancelRecreateCts()
        {
            _damageDisplayCts?.Cancel();
            _damageDisplayCts?.Dispose();
            _damageDisplayCts = new CancellationTokenSource();
        }
    }
}