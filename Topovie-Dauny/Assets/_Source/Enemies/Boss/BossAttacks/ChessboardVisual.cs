using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.BossAttacks
{
    public class ChessboardVisual
    {
        private Color _attackingTileColor;
        private Color _warningTileColor;
        private SpriteRenderer _chessboardSprite;
        
        public ChessboardVisual(SpriteRenderer chessboardSprite, Color attackingTileColor, Color warningTileColor)
        {
            _chessboardSprite = chessboardSprite;
            _attackingTileColor = attackingTileColor;
            _warningTileColor = warningTileColor;
        }
        
        private async UniTask AttackAsync(CancellationToken token)
        {
            //_chessboardSprite.gameObject.SetActive(false);
            //_chessboardSprite.color = attackingTileColor;
            //
            //await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeAttack), cancellationToken: token);
            //_chessboardSprite.gameObject.SetActive(true);
            //hitCollider.enabled = true;
            //await StopAttackAsync(token);
        }
        
        
        private async UniTask StopAttackAsync(CancellationToken token)
        {
            //await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token);
            ////hitCollider.enabled = false;
            //await _chessboardSprite.DOFade(0f, disappearTime).ToUniTask(cancellationToken: token);
            //_chessboardSprite.gameObject.SetActive(false);
            //await _chessboardSprite.DOFade(1f, 0f).ToUniTask(cancellationToken: token);
            //_chessboardSprite.color = warningTileColor;
        }
        private async UniTask ShowAttackWarningAsync(CancellationToken token)
        {
            await FadeInAsync(token);
            await BlinkAsync(token);
        }
        private async UniTask FadeInAsync(CancellationToken token)
        {
            _chessboardSprite.gameObject.SetActive(true);
            //await _chessboardSprite.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            //await _chessboardSprite.DOFade(1f, fadeInDuration).ToUniTask(cancellationToken: token);
        }
        private async UniTask BlinkAsync(CancellationToken token)
        {
            // var halfBlinkDuration = _blinkDuration / 2;
            // for (var i = 0; i < warningBlinkAmount; i++)
            // {
            //     await _chessboardSprite.DOFade(0f, halfBlinkDuration).ToUniTask(cancellationToken: token);
            //     await _chessboardSprite.DOFade(1f, halfBlinkDuration).ToUniTask(cancellationToken: token);
            // }
        }
    }
}