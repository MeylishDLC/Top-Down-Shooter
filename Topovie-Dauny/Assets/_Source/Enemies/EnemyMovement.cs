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
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement: MonoBehaviour
    {
        public static event Action<Vector3> OnEnemyDisappeared;
        public event Action OnAttack; 
        public event Action OnAttackStarted; 
        
        [Header("Sound")]
        [SerializeField] private EventReference moveSound;
        [SerializeField] private float soundFrequency;
        [SerializeField] private float soundDistance = 2f;

        [Header("Attack")] 
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float startAttackDuration = 1.5f;
        [SerializeField] private float remainingAttackDuration = 1.5f;
        [SerializeField] private float deathDuration = 1.5f;
        
        private EnemyHealth _enemyHealth;
        private AIPath _aiPath;
        private Rigidbody2D _rb;
        private Transform _playerTransform;
        
        private AudioManager _audioManager;
        private CancellationToken _ctOnDestroy;
        private float _timer;
        private float _initScale;
        
        private bool _isFacingRight;
        private bool _isAttacking;
        private bool _isDying;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
            _aiPath = GetComponent<AIPath>();
            _rb = GetComponent<Rigidbody2D>();
            _enemyHealth = GetComponent<EnemyHealth>();
            
            _initScale = transform.localScale.x;
            _ctOnDestroy = this.GetCancellationTokenOnDestroy();

            _enemyHealth.OnEnemyDied += Disappear;
        }
        private void OnDestroy()
        {
            _enemyHealth.OnEnemyDied -= Disappear;
        }
        private void OnEnable()
        {
            _timer = Random.Range(0f, soundFrequency);
            _aiPath.canMove = true;
        }
        private void Update()
        {
            if (_isDying)
            {
                return;
            }
            
            if (_aiPath.canMove)
            {
               HandleFlipping();
            }
            AttackPlayerInRange();
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
        private void AttackPlayerInRange()
        {
            if (_isAttacking || !_playerTransform)
            {
                return;
            }

            var distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                DoAttackAsync(_ctOnDestroy).Forget();
            }
        }
        private async UniTask DoAttackAsync(CancellationToken token)
        {
            OnAttackStarted?.Invoke();
            _isAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(startAttackDuration), cancellationToken: token);
            
            OnAttack?.Invoke();

            await UniTask.Delay(TimeSpan.FromSeconds(remainingAttackDuration), cancellationToken: token);
            _isAttacking = false;
        }
        public void SetDestination(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            var destinationSetter = GetComponent<AIDestinationSetter>();
            destinationSetter.target = _playerTransform;
        }
        public void SetMovement(bool canMove)
        {
            _aiPath.canMove = canMove;
            _rb.bodyType = canMove ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
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
        private void Disappear()
        {
            DisappearAsync(_ctOnDestroy).Forget();
        }
        private async UniTask DisappearAsync(CancellationToken token)
        {
            _isDying = true;
            await UniTask.Delay(TimeSpan.FromSeconds(deathDuration), cancellationToken: token);
            gameObject.SetActive(false);
            OnEnemyDisappeared?.Invoke(gameObject.transform.position);
            _isDying = false;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
#endif
    }
}