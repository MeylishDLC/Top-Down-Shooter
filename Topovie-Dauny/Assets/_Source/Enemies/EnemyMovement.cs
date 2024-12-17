using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies.Combat;
using FMODUnity;
using Pathfinding;
using SoundSystem;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyMovement: MonoBehaviour
    {
        [SerializeField] private EnemyHealth enemyHealth;
        
        [Header("Sound")]
        [SerializeField] private EventReference moveSound;
        [SerializeField] private float soundFrequency;
        [SerializeField] private float soundDistance = 2f;
        
        [Header("Visual")]
        [SerializeField] private Color colorOnDamageTaken;
        [SerializeField] private float colorStayDuration = 0.1f;
        [SerializeField] private float deathAnimationDuration = 0.5f;
        
        private AIPath _aiPath;
        private SpriteRenderer _enemyRenderer;
        private CancellationToken _deathCancellationToken;
        private Transform _playerTransform;
        
        private AudioManager _audioManager;
        private float _timer;
        private float _initScale;
        private bool _isFacingRight;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        private void Awake()
        {
            _aiPath = GetComponent<AIPath>();
            _enemyRenderer = GetComponent<SpriteRenderer>();
            _enemyRenderer.sortingOrder = Random.Range (0, 100);
            _deathCancellationToken = this.GetCancellationTokenOnDestroy();
            
            _initScale = transform.localScale.x;
            SubscribeOnEvents();
        }
        private void OnEnable()
        {
            _timer = Random.Range(0f, soundFrequency);
            _aiPath.canMove = true;
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
            
            if (moveSound.IsNull)
            {
                return;
            }
            _timer += Time.deltaTime;
            if (_timer >= soundFrequency)
            {
                _audioManager.PlayOneShot(moveSound, gameObject.transform.position, 
                    _playerTransform.position, soundDistance);
                _timer = 0;
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
            _aiPath.canMove = false;
            //await gameObject.transform.DOScaleX(0f, deathAnimationDuration).ToUniTask(cancellationToken: token);
            await _enemyRenderer.DOFade(0f, deathAnimationDuration).ToUniTask(cancellationToken: token);
            gameObject.SetActive(false);
            //await gameObject.transform.DOScaleX(_initScale, 0f);
            await _enemyRenderer.DOFade(1f, 0).ToUniTask(cancellationToken: token);
        }
        private void SubscribeOnEvents()
        {
            enemyHealth.OnDamageTaken += ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied += ShowEnemyDeath;
        }
        private void UnsubscribeOnEvents()
        {
            enemyHealth.OnDamageTaken -= ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied -= ShowEnemyDeath;
        }
    }
}