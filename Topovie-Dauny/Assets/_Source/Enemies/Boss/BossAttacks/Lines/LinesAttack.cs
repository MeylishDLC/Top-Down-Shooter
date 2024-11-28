using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using Enemies.Boss.BossAttacks.Lasers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Boss.BossAttacks.Lines
{
    public class LinesAttack: MonoBehaviour, IBossAttack
    {
        [SerializeField] private ContinuousAttack[] lines;
        [SerializeField] private float transparencyOnWarn;
        [SerializeField] private float transparencyOnAttack;
        [SerializeField] private BaseBossAttackConfig config;

        [SerializeField] private float delayBetweenAttacks;

        private bool _isAttackingOpposite;
        private SpriteRenderer[] _linesRenderers;
        private CancellationToken _destroyCancellationToken;
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            SetAttackEnabled(false);
            _linesRenderers = GetRenderers();
            foreach (var line in _linesRenderers)
            {
                DoLineFade(line, 0,0,_destroyCancellationToken);
            }
        }
        public async UniTask TriggerAttack(CancellationToken token)
        {
            SetRandomAttackDirection();
            var start = _isAttackingOpposite ? lines.Length - 1 : 0;
            var end = _isAttackingOpposite ? -1 : lines.Length;
            var step = _isAttackingOpposite ? -1 : 1;

            for (int i = start; i != end; i += step)
            {
                TriggerLineAttack(i, _destroyCancellationToken).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAttacks), cancellationToken: _destroyCancellationToken);
            }
        }

        private void SetRandomAttackDirection()
        {
            _isAttackingOpposite = Random.Range(0, 2) == 0;
        }
        private UniTask TriggerLineAttack(int lineIndex, CancellationToken token)
        {
            return ShowWarnLineAsync(lineIndex, token)
                .ContinueWith(() => DoLineAttack(lineIndex, token))
                .ContinueWith(() => StopLineAttack(lineIndex, token));
        }
        private async UniTask ShowWarnLineAsync(int lineIndex, CancellationToken token)
        {
            await DoLineFade(_linesRenderers[lineIndex], transparencyOnWarn, config.FadeInTime, token);
            await UniTask.Delay(TimeSpan.FromSeconds(config.WarningDuration), cancellationToken: token);
        }

        private async UniTask DoLineAttack(int lineIndex, CancellationToken token)
        {
            await DoLineFade(_linesRenderers[lineIndex], transparencyOnAttack, config.TransitionDuration, token);
            lines[lineIndex].enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(config.AttackDuration), cancellationToken: token);
        }

        private async UniTask StopLineAttack(int lineIndex, CancellationToken token)
        {
            lines[lineIndex].enabled = false;
            await DoLineFade(_linesRenderers[lineIndex], 0, config.FadeOutTime, token);
        }
        private void SetAttackEnabled(bool enable)
        {
            foreach (var lineAttack in lines)
            {
                lineAttack.enabled = enable;
            }
        }
        private UniTask DoLineFade(SpriteRenderer line, float endValue, float duration, CancellationToken token)
        {
            return line.DOFade(endValue, duration).ToUniTask(cancellationToken: token);
        }
        private SpriteRenderer[] GetRenderers()
        {
            return lines.Select(line => line.gameObject.GetComponent<SpriteRenderer>()).ToArray();
        }
    }
}