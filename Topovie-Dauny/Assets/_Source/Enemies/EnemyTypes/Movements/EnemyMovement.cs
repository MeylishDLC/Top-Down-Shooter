using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Pathfinding;
using SoundSystem;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemies.EnemyTypes.Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement: MonoBehaviour
    {
        public static event Action<Vector3> OnEnemyDisappeared;
        public Action OnAttack; 
        public Action OnAttackStarted; 
        
        [SerializeField] protected EnemyConfig config;
        
        protected float Timer;
        protected AIPath AIPath;
        protected Transform PlayerTransform;
        protected AudioManager AudioManager;
        protected CancellationToken CtOnDestroy;
        
        private EnemyHealth _enemyHealth;
        private Rigidbody2D _rb;
        
        private float _initScale;
        private bool _isFacingRight;
        private bool _isAttacking;
        protected bool IsDying;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            AudioManager = audioManager;
            AIPath = GetComponent<AIPath>();
            _rb = GetComponent<Rigidbody2D>();
            _enemyHealth = GetComponent<EnemyHealth>();
            
            _initScale = transform.localScale.x;
            CtOnDestroy = this.GetCancellationTokenOnDestroy();

            _enemyHealth.OnEnemyDied += Disappear;
        }
        protected virtual void OnDestroy()
        {
            _enemyHealth.OnEnemyDied -= Disappear;
        }
        private void OnEnable()
        {
            Timer = Random.Range(0f, config.SoundFrequency);
            AIPath.canMove = true;
            SetMovement(true);
        }
        protected virtual void Update()
        {
            if (IsDying)
            {
                return;
            }
            
            if (AIPath.canMove)
            {
               HandleFlipping();
            }
            AttackPlayerInRange();
            if (config.MoveSound.IsNull)
            {
                return;
            }
            Timer += Time.deltaTime;
            if (Timer >= config.SoundFrequency)
            {
                AudioManager.PlayOneShot(config.MoveSound, gameObject.transform.position, 
                    PlayerTransform.position, config.SoundDistance);
                Timer = 0;
            }
        }
        private void AttackPlayerInRange()
        {
            if (_isAttacking || !PlayerTransform)
            {
                return;
            }

            var distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);
            if (distanceToPlayer <= config.AttackRange)
            {
                DoAttackAsync(CtOnDestroy).Forget();
            }
        }
        private async UniTask DoAttackAsync(CancellationToken token)
        {
            OnAttackStarted?.Invoke();
            _isAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(config.StartAttackDuration), cancellationToken: token);
            
            OnAttack?.Invoke();

            await UniTask.Delay(TimeSpan.FromSeconds(config.RemainingAttackDuration), cancellationToken: token);
            _isAttacking = false;
        }
        public void SetDestination(Transform playerTransform)
        {
            PlayerTransform = playerTransform;
            var destinationSetter = GetComponent<AIDestinationSetter>();
            destinationSetter.target = PlayerTransform;
        }
        public void SetMovement(bool canMove)
        {
            AIPath.canMove = canMove;
            _rb.bodyType = canMove ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }

        protected void HandleFlipping()
        {
            var directionToTarget = PlayerTransform.position.x - transform.position.x;
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
            DisappearAsync(CtOnDestroy).Forget();
        }
        private async UniTask DisappearAsync(CancellationToken token)
        {
            IsDying = true;
            SetMovement(false);
            await UniTask.Delay(TimeSpan.FromSeconds(config.DeathDuration), cancellationToken: token);
            gameObject.SetActive(false);
            OnEnemyDisappeared?.Invoke(gameObject.transform.position);
            IsDying = false;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.AttackRange);
        }
#endif
    }
}