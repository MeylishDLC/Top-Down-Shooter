using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoundSystem;
using UnityEngine;

namespace Enemies.Boss.BossAttacks.Lasers
{
    public class LasersVisual
    {
        public event Action OnAttackStarted;
        public event Action OnAttackEnded;
        
        private static readonly int LaserIntensity = Shader.PropertyToID("_LaserIntensity");

        private readonly Material _laserMaterial;
        private readonly float _intensityOnWarn;
        private readonly float _intensityOnAttack;

        private readonly float _fadeInTime;
        private readonly float _fadeOutTime;
        private readonly float _transitionDuration;
        private readonly float _warningDuration;
        private readonly float _attackDuration;
        private readonly AudioManager _audioManager;
        public LasersVisual(BaseBossAttackConfig config, Material laserMaterial, 
            float intensityOnWarn, float intensityOnAttack, AudioManager audioManager)
        {
            _laserMaterial = laserMaterial;
            _intensityOnWarn = intensityOnWarn;
            _intensityOnAttack = intensityOnAttack;
            
            _fadeInTime = config.FadeInTime;
            _fadeOutTime = config.FadeOutTime;
            _transitionDuration = config.TransitionDuration;
            _warningDuration = config.WarningDuration;
            _attackDuration = config.AttackDuration;
            _audioManager = audioManager;
        }
        public async UniTask ShowWarningAsync(CancellationToken token)
        {
            await DoLasersFade(_intensityOnWarn, _fadeInTime,token);
            await UniTask.Delay(TimeSpan.FromSeconds(_warningDuration), cancellationToken: token);
        }

        public async UniTask ShowStartAttackAsync(CancellationToken token)
        {
            _audioManager.PlayOneShot(_audioManager.FMODEvents.LasersSound);
            await DoLasersFade(_intensityOnAttack, _transitionDuration, token);
            OnAttackStarted?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDuration), cancellationToken: token);
        }

        public UniTask ShowStopAttack(CancellationToken token)
        {
            OnAttackEnded?.Invoke();
            return DoLasersFade(0, _fadeOutTime, token);
        }
        public UniTask DoLasersFade(float endIntensity, float duration, CancellationToken token)
        {
            return DOTween.To
            (() => _laserMaterial.GetFloat(LaserIntensity), SetLightIntensity, 
                endIntensity, duration).ToUniTask(cancellationToken: token);
        }
        
        private void SetLightIntensity(float intensity)
        {
            _laserMaterial.SetFloat(LaserIntensity, intensity);
        }
    }
}