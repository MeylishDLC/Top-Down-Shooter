using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Boss.BossAttacks
{
    public class ChessboardAttack : MonoBehaviour
    {
        [Header("Visuals")] 
        [SerializeField] private SpriteRenderer chessboardSprite;
        [SerializeField] private Collider2D hitCollider;
        
        [Header("Colors")]
        [SerializeField] private Color warningTileColor;
        [SerializeField] private Color attackingTileColor = Color.white;

        [Header("Warning Time Settings")] 
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float warningDuration;
        [SerializeField] private int warningBlinkAmount;
        
        [Header("Attack Time Settings")]
        [SerializeField] private float attackDuration;
        [SerializeField] private float delayBeforeAttack;
        [SerializeField] private float attackRate;
        [SerializeField] private float disappearTime;

        private float _blinkDuration;
        private CancellationToken _destroyCancellationToken;
        private ChessboardVisual _chessboardVisual;
        
        private void Awake()
        {
            _blinkDuration = warningDuration/warningBlinkAmount;
            
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            hitCollider.enabled = false;
            chessboardSprite.color = warningTileColor;
            chessboardSprite.gameObject.SetActive(false);
        }

        public UniTask TriggerAttack(CancellationToken token)
        {
            return ShowAttackWarningAsync(_destroyCancellationToken).ContinueWith(() => AttackAsync(_destroyCancellationToken));
        }
        private async UniTask AttackAsync(CancellationToken token)
        {
            chessboardSprite.gameObject.SetActive(false);
            chessboardSprite.color = attackingTileColor;
            
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeAttack), cancellationToken: token);
            chessboardSprite.gameObject.SetActive(true);
            hitCollider.enabled = true;
            await StopAttackAsync(token);
        }
        private async UniTask StopAttackAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token);
            hitCollider.enabled = false;
            await chessboardSprite.DOFade(0f, disappearTime).ToUniTask(cancellationToken: token);
            chessboardSprite.gameObject.SetActive(false);
            await chessboardSprite.DOFade(1f, 0f).ToUniTask(cancellationToken: token);
            chessboardSprite.color = warningTileColor;
        }
        private async UniTask ShowAttackWarningAsync(CancellationToken token)
        {
            await FadeInAsync(token);
            await BlinkAsync(token);
        }
        private async UniTask FadeInAsync(CancellationToken token)
        {
            chessboardSprite.gameObject.SetActive(true);
            await chessboardSprite.DOFade(0f, 0f).ToUniTask(cancellationToken: token);
            await chessboardSprite.DOFade(1f, fadeInDuration).ToUniTask(cancellationToken: token);
        }
        private async UniTask BlinkAsync(CancellationToken token)
        {
            var halfBlinkDuration = _blinkDuration / 2;
            for (var i = 0; i < warningBlinkAmount; i++)
            {
                await chessboardSprite.DOFade(0f, halfBlinkDuration).ToUniTask(cancellationToken: token);
                await chessboardSprite.DOFade(1f, halfBlinkDuration).ToUniTask(cancellationToken: token);
            }
        }
    }
}
