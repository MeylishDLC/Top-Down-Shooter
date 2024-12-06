using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoundSystem;
using UnityEngine;
using Zenject;
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
        private CancellationToken _destroyCancellationToken;
        private LinesAttackVisual _linesAttackVisual;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _linesAttackVisual = new LinesAttackVisual(config, lines, transparencyOnWarn, transparencyOnAttack, audioManager);
        }
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            foreach (var line in lines)
            {
                line.enabled = false;
            }
            
            _linesAttackVisual.SetAllLinesTransparency(0);
            _linesAttackVisual.OnLineAttackStarted += EnableAttack;
            _linesAttackVisual.OnLineAttackEnded += DisableAttack;
        }
        private void OnDestroy()
        {
            _linesAttackVisual.OnLineAttackStarted -= EnableAttack;
            _linesAttackVisual.OnLineAttackEnded -= DisableAttack;
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
            return _linesAttackVisual.ShowLineWarnAsync(lineIndex, token)
                .ContinueWith(() => _linesAttackVisual.ShowLineAttack(lineIndex, token))
                .ContinueWith(() => _linesAttackVisual.ShowLineStopAttack(lineIndex, token));
        }
        private void EnableAttack(int index)
        {
            lines[index].enabled = true;
        }
        private void DisableAttack(int index)
        {
            lines[index].enabled = false;
        }
    }
}