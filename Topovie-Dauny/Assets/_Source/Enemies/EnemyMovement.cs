using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies.Combat;
using Pathfinding;
using UnityEngine;

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

        private float _initScale;
        private bool _isFacingRight;
        private void Awake()
        {
            _aiPath = GetComponent<AIPath>();
            _enemyRenderer = GetComponent<SpriteRenderer>();
            _deathCancellationToken = this.GetCancellationTokenOnDestroy();
            
            _knockBack = enemyHealth.KnockBack;
            _initScale = transform.localScale.x;
            SubscribeOnEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeOnEvents();
        }
        private void Update()
        {
            if (_aiPath.canMove)
            {
               HandleFlipping();
            }
        }
        public void SetDestination(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            var destinationSetter = GetComponent<AIDestinationSetter>();
            destinationSetter.target = _playerTransform;
        }
        private void HandleFlipping()
        {
            var directionToTarget = _playerTransform.position.x - transform.position.x;
            if (directionToTarget > 0 && !_isFacingRight)
            {
                Flip(); 
            }
            else if (directionToTarget < 0 && _isFacingRight)
            {
                Flip(); 
            }
        }
        private void Flip()
        {
            _isFacingRight = !_isFacingRight; 
            var newScaleX = _isFacingRight ? -_initScale : _initScale;
            transform.DOScaleX(newScaleX, 0f);
        }
        private void EnableMovement()
        {
            _aiPath.canMove = true;
        }
        private void DisableMovement()
        {
            _aiPath.canMove = false;
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
            gameObject.SetActive(false);
            await gameObject.transform.DOScaleX(_initScale, 0f);
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
            if (_knockBack is null)
            {
                return;
            }
            _knockBack.OnKnockBackStarted -= DisableMovement;
            _knockBack.OnKnockBackEnded -= EnableMovement;
            enemyHealth.OnDamageTaken -= ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied -= ShowEnemyDeath;
        }
    }
}