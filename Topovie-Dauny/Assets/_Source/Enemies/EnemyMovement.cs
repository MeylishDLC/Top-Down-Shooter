using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies.Combat;
using Pathfinding;
using Player.PlayerControl;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Enemies
{
    public class EnemyMovement: MonoBehaviour
    {
        [SerializeField] private EnemyHealth enemyHealth;
        
        [Header("Visual")]
        [SerializeField] private Color colorOnDamageTaken;
        [SerializeField] private float colorStayDuration = 0.1f;
        [SerializeField] private float deathAnimationDuration = 0.5f;

        private AIPath _aiPath;
        private SpriteRenderer _enemyRenderer;
        private KnockBack _knockBack;
        private CancellationToken _deathCancellationToken;
        private Transform _playerTransform;
        
        private void Start()
        {
            _playerTransform = enemyHealth.PlayerMovement.transform;

            var destinationSetter = GetComponent<AIDestinationSetter>();
            destinationSetter.target = _playerTransform;
            _aiPath = GetComponent<AIPath>();
            _enemyRenderer = GetComponent<SpriteRenderer>();
            _deathCancellationToken = this.GetCancellationTokenOnDestroy();
            
            _knockBack = enemyHealth.KnockBack;
            SubscribeOnEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeOnEvents();
        }
        private void EnableMovement()
        {
            _aiPath.enabled = true;
        }
        private void DisableMovement()
        {
            _aiPath.enabled = false;
        }
        private void ShowEnemyDeath()
        {
            ShowEnemyDeathAsync(_deathCancellationToken).Forget();
        }
        private void ChangeColorOnDamageTaken()
        {
            ChangeColorOnDamageTakenAsync(_deathCancellationToken).Forget();
        }
        private async UniTask ChangeColorOnDamageTakenAsync(CancellationToken token)
        {
            _enemyRenderer.color = colorOnDamageTaken;
            await UniTask.Delay(TimeSpan.FromSeconds(colorStayDuration), cancellationToken: token);
            _enemyRenderer.color = Color.white;
        }
        private async UniTask ShowEnemyDeathAsync(CancellationToken token)
        {
            DisableMovement();
            await gameObject.transform.DOScaleX(0f, deathAnimationDuration).ToUniTask(cancellationToken: token);
            Destroy(gameObject);
        }
        private void SubscribeOnEvents()
        {
            _knockBack.OnKnockBackStarted += DisableMovement;
            _knockBack.OnKnockBackEnded += EnableMovement;
            enemyHealth.OnDamageTaken += ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied += ShowEnemyDeath;
        }
        private void UnsubscribeOnEvents()
        {
            _knockBack.OnKnockBackStarted -= DisableMovement;
            _knockBack.OnKnockBackEnded -= EnableMovement;
            enemyHealth.OnDamageTaken -= ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied -= ShowEnemyDeath;
        }
    }
}