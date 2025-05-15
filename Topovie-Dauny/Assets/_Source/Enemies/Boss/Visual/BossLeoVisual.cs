using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.Visual
{
    public class BossLeoVisual
    {
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Sprite _attackSprite;
        private readonly Sprite _vulnerableSprite;
        private readonly float _hurtDuration;
        private CancellationToken _ct;
        public BossLeoVisual(SpriteRenderer spriteRenderer, Sprite attackSprite, Sprite vulnerableSprite, float hurtDuration,
            CancellationToken ct)
        {
            _spriteRenderer = spriteRenderer;
            _attackSprite = attackSprite;
            _vulnerableSprite = vulnerableSprite;
            _hurtDuration = hurtDuration;
            _spriteRenderer.sprite = _attackSprite;
            _ct = ct;
        }

        public void ShowLeoHurt()
        {
            ShowLeoHurtAsync(_ct).Forget();
        }

        public void SetLeoHurt()
        {
            _spriteRenderer.sprite = _vulnerableSprite;
        }
        private async UniTask ShowLeoHurtAsync(CancellationToken token)
        {
            _spriteRenderer.sprite = _vulnerableSprite;
            await UniTask.Delay(TimeSpan.FromSeconds(_hurtDuration), cancellationToken: token);
            _spriteRenderer.sprite = _attackSprite;
        }
    }
}