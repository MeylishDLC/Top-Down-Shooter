using System;
using System.Linq;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.BossAttacks.Lines
{
    public class LinesAttackVisual
    {
        public event Action<int> OnLineAttackStarted;
        public event Action<int> OnLineAttackEnded;
            
        private readonly SpriteRenderer[] _linesRenderers;
        private readonly float _transparencyOnWarn;
        private readonly float _transparencyOnAttack;

        private readonly float _fadeInTime;
        private readonly float _fadeOutTime;
        private readonly float _transitionDuration;
        private readonly float _warningDuration;
        private readonly float _attackDuration;
        
        public LinesAttackVisual(BaseBossAttackConfig config, ContinuousAttack[] lines, 
            float transparencyOnWarn, float transparencyOnAttack)
        {
            _linesRenderers = GetRenderers(lines);
            _transparencyOnAttack = transparencyOnAttack;
            _transparencyOnWarn = transparencyOnWarn;
            
            _fadeInTime = config.FadeInTime;
            _fadeOutTime = config.FadeOutTime;
            _transitionDuration = config.TransitionDuration;
            _warningDuration = config.WarningDuration;
            _attackDuration = config.AttackDuration;
        }
        
        public async UniTask ShowLineWarnAsync(int lineIndex, CancellationToken token)
        {
            await DoLineFade(_linesRenderers[lineIndex], _transparencyOnWarn, _fadeInTime, token);
            await UniTask.Delay(TimeSpan.FromSeconds(_warningDuration), cancellationToken: token);
        }

        public async UniTask ShowLineAttack(int lineIndex, CancellationToken token)
        {
            await DoLineFade(_linesRenderers[lineIndex], _transparencyOnAttack, _transitionDuration, token);
            OnLineAttackStarted?.Invoke(lineIndex);
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDuration), cancellationToken: token);
        }

        public async UniTask ShowLineStopAttack(int lineIndex, CancellationToken token)
        {
            OnLineAttackEnded?.Invoke(lineIndex);
            await DoLineFade(_linesRenderers[lineIndex], 0, _fadeOutTime, token);
        }

        public void SetAllLinesTransparency(float value)
        {
            foreach (var line in _linesRenderers)
            {
                line.DOFade(value, 0f);
            }
        }
        private UniTask DoLineFade(SpriteRenderer line, float endValue, float duration, CancellationToken token)
        {
            return line.DOFade(endValue, duration).ToUniTask(cancellationToken: token);
        }
        private SpriteRenderer[] GetRenderers(ContinuousAttack[] lines)
        {
            return lines.Select(line => line.gameObject.GetComponent<SpriteRenderer>()).ToArray();
        }
    }
}