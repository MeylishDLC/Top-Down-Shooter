using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Boss.Phases;
using UnityEngine;

namespace Enemies.Boss
{
    public class BossLeoVisual
    {
        private readonly SpriteRenderer _headRenderer;
        private readonly Sprite _attackSprite;
        private readonly Sprite _vulnerableSprite;
        private readonly float _hurtDuration;
        private CancellationToken _ct;
        public BossLeoVisual(SpriteRenderer headRenderer, Sprite attackSprite, Sprite vulnerableSprite, float hurtDuration,
            CancellationToken ct)
        {
            _headRenderer = headRenderer;
            _attackSprite = attackSprite;
            _vulnerableSprite = vulnerableSprite;
            _hurtDuration = hurtDuration;
            _headRenderer.sprite = _attackSprite;
            _ct = ct;
        }

        public void ShowLeoHurt()
        {
            ShowLeoHurtAsync(_ct).Forget();
        }

        public void SetLeoHurt()
        {
            _headRenderer.sprite = _vulnerableSprite;
        }
        private async UniTask ShowLeoHurtAsync(CancellationToken token)
        {
            _headRenderer.sprite = _vulnerableSprite;
            await UniTask.Delay(TimeSpan.FromSeconds(_hurtDuration), cancellationToken: token);
            _headRenderer.sprite = _attackSprite;
        }
    }
}