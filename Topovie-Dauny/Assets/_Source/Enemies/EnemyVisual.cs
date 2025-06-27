using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using Enemies.EnemyTypes;
using Enemies.EnemyTypes.Movements;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyVisual: MonoBehaviour
    {
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private EnemyMovement enemyMovement;
        [SerializeField] private EnemyConfig config;

        [Header("Visual")]
        [SerializeField] private Color colorOnDamageTaken = Color.red;
        [SerializeField] private float colorStayDuration = 0.1f;
        
        private static readonly int AttackProperty = Animator.StringToHash("onAttack");
        private static readonly int OnDeath = Animator.StringToHash("onDeath");

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private CancellationToken _ctOnDeath;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _spriteRenderer.sortingOrder = Random.Range (0, 100);
            SubscribeOnEvents();
        }
        private void OnEnable()
        {
            _spriteRenderer.DOFade(1f, 0f);
        }
        private void OnDestroy()
        {
            UnsubscribeOnEvents();
        }
        private void TriggerAttackAnimation()
        {
            _animator.SetTrigger(AttackProperty);
        }
        private void TriggerDeathAnimation()
        {
            FadeOnDeath(_ctOnDeath).Forget();
        }
        private void ChangeColorOnDamageTaken()
        {
           ChangeColorOnDamageTakenAsync(_ctOnDeath).Forget();
        }
        private async UniTask ChangeColorOnDamageTakenAsync(CancellationToken token)
        {
            _spriteRenderer.color = colorOnDamageTaken;
            await UniTask.Delay(TimeSpan.FromSeconds(colorStayDuration), cancellationToken: token);
            _spriteRenderer.color = Color.white;
        }

        private async UniTask FadeOnDeath(CancellationToken token)
        {
            _animator.SetTrigger(OnDeath);
            await _spriteRenderer.DOFade(0f, config.DeathDuration).ToUniTask(cancellationToken: token);
        }
        private void SubscribeOnEvents()
        {
            enemyHealth.OnDamageTaken += ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied += TriggerDeathAnimation;

            enemyMovement.OnAttackStarted += TriggerAttackAnimation;
        }
        private void UnsubscribeOnEvents()
        {
            enemyHealth.OnDamageTaken -= ChangeColorOnDamageTaken;
            enemyHealth.OnEnemyDied -= TriggerDeathAnimation;
            
            enemyMovement.OnAttackStarted -= TriggerAttackAnimation;
        }
    }
}