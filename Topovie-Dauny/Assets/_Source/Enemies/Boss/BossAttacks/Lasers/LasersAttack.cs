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
        [SerializeField] private BaseBossAttackConfig config;

        private CancellationToken _destroyCancellationToken;
        private LasersVisual _lasersVisual;
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            DisableAttack();
            _lasersVisual = new LasersVisual(config, laserMaterial, intensityOnWarn, intensityOnAttack);
            _lasersVisual.DoLasersFade(0,0, _destroyCancellationToken);

            _lasersVisual.OnAttackEnded += DisableAttack;
            _lasersVisual.OnAttackStarted += EnableAttack;
        }
        private void OnDestroy()
        {
            _lasersVisual.OnAttackEnded -= DisableAttack;
            _lasersVisual.OnAttackStarted -= EnableAttack;
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            return _lasersVisual.ShowWarningAsync(_destroyCancellationToken)
                .ContinueWith(() => _lasersVisual.ShowStartAttackAsync(_destroyCancellationToken))
                .ContinueWith(() => _lasersVisual.ShowStopAttack(_destroyCancellationToken));
        }
        private void EnableAttack()
        {
            foreach (var laserAttack in lasers)
            {
                laserAttack.enabled = true;
            }
        }
        private void DisableAttack()
        {
            foreach (var laserAttack in lasers)
            {
                laserAttack.enabled = false;
            }
        }
    }
}