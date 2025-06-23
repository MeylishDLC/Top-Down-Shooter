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
        
        [Header("Sound")]
        [SerializeField]
        protected EventReference moveSound;
        [SerializeField] protected float soundFrequency;
        [SerializeField] protected float soundDistance = 2f;

        [Header("Attack")] 
        [SerializeField]
        protected float attackRange = 1.5f;
        [SerializeField] protected float startAttackDuration = 1.5f;
        [SerializeField] protected float remainingAttackDuration = 1.5f;
        [SerializeField] private float deathDuration = 1.5f;
        
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
            Timer = Random.Range(0f, soundFrequency);
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
            if (moveSound.IsNull)
            {
                return;
            }
            Timer += Time.deltaTime;
            if (Timer >= soundFrequency)
            {
                AudioManager.PlayOneShot(moveSound, gameObject.transform.position, 
                    PlayerTransform.position, soundDistance);
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
            if (distanceToPlayer <= attackRange)
            {
                DoAttackAsync(CtOnDestroy).Forget();
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
            await UniTask.Delay(TimeSpan.FromSeconds(deathDuration), cancellationToken: token);
            gameObject.SetActive(false);
            OnEnemyDisappeared?.Invoke(gameObject.transform.position);
            IsDying = false;
        }

        private async UniTask FadeAndDisappearAsync(CancellationToken token)
        {
            IsDying = true;
            SetMovement(false);
             
            await UniTask.Delay(TimeSpan.FromSeconds(deathDuration), cancellationToken: token);
            gameObject.SetActive(false);
            OnEnemyDisappeared?.Invoke(gameObject.transform.position);
            IsDying = false;
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