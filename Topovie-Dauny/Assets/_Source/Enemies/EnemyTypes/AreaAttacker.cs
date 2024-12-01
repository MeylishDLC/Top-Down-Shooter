using System;
using System.Threading;
using System.Threading.Tasks;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using Pathfinding;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Enemies.EnemyTypes
{
    public class AreaAttacker: MonoBehaviour
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private EnemyHealth enemyHealth;
        
        [Header("Attack Settings")]
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackDuration;
        [SerializeField] private Animator blowAnimator;
        [SerializeField] private Color attackColor = Color.red;
        [SerializeField] private float colorTransitionDuration;
        
        [Header("Warn Settings")]
        [SerializeField] private float warningDuration;
        [SerializeField] private SpriteRenderer rangeSprite;
        [SerializeField] private float rangeTransparencyOnWarn;
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float fadeOutDuration;

        private static readonly int AttackTrigger = Animator.StringToHash("attack");
        private CancellationTokenSource _cancelAttackCts = new();
        private CancellationToken _destroyCancellationToken;

        private Color _initRangeColor;
        private PlayerHealth _playerHealth;
        private bool _isPlayerInRange;
        private bool _isWarning;
        
        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _playerHealth = enemyHealth.PlayerMovement.GetComponent<PlayerHealth>();
            _initRangeColor = rangeSprite.color;
            rangeSprite.DOFade(0f, 0f);
        }
        private void OnDestroy()
        {
            _cancelAttackCts?.Cancel();
            _cancelAttackCts?.Dispose();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = true;
                if (!_isWarning)
                {
                    WarnAsync(_cancelAttackCts.Token).Forget();
                }
            }
        }
        private async void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = false;
                if (_isWarning && aiPath.enabled)
                {
                    _isWarning = false;
                    CancelRecreateCts();
                    await FadeArea(0f, fadeOutDuration, _destroyCancellationToken);
                }
            }
        }

        private async UniTask WarnAsync(CancellationToken token)
        {
            try
            {
                _isWarning = true;
                await FadeArea(rangeTransparencyOnWarn, fadeInDuration, token);
                await UniTask.Delay(TimeSpan.FromSeconds(warningDuration), cancellationToken: token);
                _isWarning = false;
                await AttackAsync(_destroyCancellationToken);
            }
            catch (TaskCanceledException)
            {
                //
            }
        }
        private UniTask FadeArea(float value, float duration, CancellationToken token)
        {
            return rangeSprite.DOFade(value, duration).ToUniTask(cancellationToken: token);
        }
        private async UniTask AttackAsync(CancellationToken token)
        {
            aiPath.enabled = false;
            await ShowStartAttackAsync(token);
            if (_isPlayerInRange)
            {
                _playerHealth.TakeDamageWithKnockback(attackDamage, transform);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token);
            await ShowEndAttackAsync(token);
            aiPath.enabled = true;
        }

        private async UniTask ShowStartAttackAsync(CancellationToken token)
        {
            blowAnimator.SetTrigger(AttackTrigger);
            await rangeSprite.DOColor(attackColor, colorTransitionDuration).ToUniTask(cancellationToken: token);
        }

        private async UniTask ShowEndAttackAsync(CancellationToken token)
        {
            await rangeSprite.DOColor(_initRangeColor, colorTransitionDuration).ToUniTask(cancellationToken: token);
            await FadeArea(0f, fadeOutDuration, token);
        }
        private void CancelRecreateCts()
        {
            _cancelAttackCts?.Cancel();
            _cancelAttackCts?.Dispose();
            _cancelAttackCts = new CancellationTokenSource();
        }
    }
}