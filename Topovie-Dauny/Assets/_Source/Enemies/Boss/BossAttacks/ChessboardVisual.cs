using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.BossAttacks
{
    public class ChessboardVisual
    {
        public event Action OnAttackStarted;
        public event Action OnAttackEnded;
        
        private readonly SpriteRenderer _chessboardSprite;
        private readonly Color _attackingTileColor;
        private readonly Color _warningTileColor;

        private readonly float _delayBeforeAttack;
        private readonly float _attackDuration;
        private readonly float _fadeOutDuration;
        private readonly float _fadeInDuration;
        private readonly float _blinkDuration;
        private readonly int _warningBlinkAmount;
        
        public ChessboardVisual(SpriteRenderer chessboardSprite, ChessboardConfig config)
        {
            _chessboardSprite = chessboardSprite;
            _attackingTileColor = config.AttackingTileColor;
            _warningTileColor = config.WarningTileColor;

            _delayBeforeAttack = config.DelayBeforeAttack;
            _attackDuration = config.AttackDuration;
            _fadeOutDuration = config.FadeOutDuration;
            _fadeInDuration = config.FadeInDuration;
            
            _warningBlinkAmount = config.WarningBlinkAmount;
            _blinkDuration = config.WarningDuration/config.WarningBlinkAmount;
        }
        public async UniTask StartAttackAsync(CancellationToken token)
        {
            _chessboardSprite.gameObject.SetActive(false);
            _chessboardSprite.color = _attackingTileColor;
            
            await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeAttack), cancellationToken: token);
            _chessboardSprite.gameObject.SetActive(true);
            OnAttackStarted?.Invoke();
            
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDuration), cancellationToken: token);
            await StopAttackAsync(token);
        }
        public async UniTask ShowAttackWarningAsync(CancellationToken token)
        {
            await FadeInAsync(token);
            await BlinkAsync(token);
        }
        private async UniTask StopAttackAsync(CancellationToken token)
        {
            OnAttackEnded?.Invoke();
            await FadeOutAsync(token);
        }
        private async UniTask FadeInAsync(CancellationToken token)
        {
            _chessboardSprite.gameObject.SetActive(true);
            await _chessboardSprite.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await _chessboardSprite.DOFade(1f, _fadeInDuration).ToUniTask(cancellationToken: token);
        }
        private async UniTask FadeOutAsync(CancellationToken token)
        {
            await _chessboardSprite.DOFade(0f, _fadeOutDuration).ToUniTask(cancellationToken: token);
            _chessboardSprite.gameObject.SetActive(false);
            await _chessboardSprite.DOFade(1f, 0f).ToUniTask(cancellationToken: token);
            _chessboardSprite.color = _warningTileColor;
        }
        private async UniTask BlinkAsync(CancellationToken token)
        {
            var halfBlinkDuration = _blinkDuration / 2;
            for (var i = 0; i < _warningBlinkAmount; i++)
            {
                await _chessboardSprite.DOFade(0f, halfBlinkDuration).ToUniTask(cancellationToken: token);
                await _chessboardSprite.DOFade(1f, halfBlinkDuration).ToUniTask(cancellationToken: token);
            }
        }
    }
}