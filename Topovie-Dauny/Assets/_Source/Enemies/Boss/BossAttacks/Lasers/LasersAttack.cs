using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Enemies.Boss.BossAttacks.Lasers
{
    public class LasersAttack: MonoBehaviour, IBossAttack
    {
        [SerializeField] private List<ContinuousAttack> lasers;
        [SerializeField] private Material laserMaterial;
        [SerializeField] private float intensityOnWarn;
        [SerializeField] private float intensityOnAttack;
        
        [Header("Timing Settings")]
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float warningDuration;
        [SerializeField] private float transitionDuration;
        [SerializeField] private float attackDuration;
        [SerializeField] private float fadeOutDuration;

        private CancellationToken _destroyCancellationToken;
        private static readonly int LaserIntensity = Shader.PropertyToID("_LaserIntensity");
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            SetLasersAttack(false);
            DoLasersFade(0, 0, _destroyCancellationToken);
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            return ShowWarningAsync(_destroyCancellationToken)
                .ContinueWith(() => StartAttackAsync(_destroyCancellationToken))
                .ContinueWith(() => StopAttack(_destroyCancellationToken));
        }
        private async UniTask ShowWarningAsync(CancellationToken token)
        {
            await DoLasersFade(intensityOnWarn, fadeInDuration,token);
            await UniTask.Delay(TimeSpan.FromSeconds(warningDuration), cancellationToken: token);
        }

        private async UniTask StartAttackAsync(CancellationToken token)
        {
            await DoLasersFade(intensityOnAttack, transitionDuration, token);
            SetLasersAttack(true);
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token);
        }

        private UniTask StopAttack(CancellationToken token)
        {
            SetLasersAttack(false);
            return DoLasersFade(0, fadeOutDuration, token);
        }
        private void SetLasersAttack(bool enable)
        {
            foreach (var attack in lasers)
            {
                attack.enabled = enable;
            }
        }
        private UniTask DoLasersFade(float endIntensity, float duration, CancellationToken token)
        {
            return DOTween.To
            (() => laserMaterial.GetFloat(LaserIntensity), SetLightIntensity, 
                endIntensity, duration).ToUniTask(cancellationToken: token);
        }
        
        private void SetLightIntensity(float intensity)
        {
            laserMaterial.SetFloat(LaserIntensity, intensity);
        }
    }
}