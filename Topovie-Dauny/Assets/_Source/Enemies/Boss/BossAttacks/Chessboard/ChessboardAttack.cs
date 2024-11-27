using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Enemies.Boss.BossAttacks.Chessboard
{
    public class ChessboardAttack : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer chessboardSprite;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private ChessboardConfig config;

        private CancellationToken _destroyCancellationToken;
        private ChessboardVisual _chessboardVisual;
        
        private void Awake()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _chessboardVisual = new ChessboardVisual(chessboardSprite, config);
            _chessboardVisual.OnAttackStarted += EnableHitCollider;
            _chessboardVisual.OnAttackEnded += DisableHitCollider;
            
            hitCollider.enabled = false;
            chessboardSprite.color = config.WarningTileColor;
            chessboardSprite.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            _chessboardVisual.OnAttackStarted -= EnableHitCollider;
            _chessboardVisual.OnAttackEnded -= DisableHitCollider;
        }
        public UniTask TriggerAttack(CancellationToken token)
        {
            return _chessboardVisual.ShowAttackWarningAsync(_destroyCancellationToken)
                .ContinueWith(() => _chessboardVisual.StartAttackAsync(_destroyCancellationToken));
        }
        private void EnableHitCollider()
        {
            hitCollider.enabled = true;
        }
        private void DisableHitCollider()
        {
            hitCollider.enabled = false;
        }
    }
}
