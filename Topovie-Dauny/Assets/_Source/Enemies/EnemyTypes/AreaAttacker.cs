using System;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cinemachine;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Pathfinding;
using Player.PlayerCombat;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Enemies.EnemyTypes
{
    public class AreaAttacker: MonoBehaviour
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private ParticleSystem impactParticlesPrefab;
        
        [Header("Attack Settings")]
        [SerializeField] private float impulseStrength;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackDuration;
        [SerializeField] private Color attackColor = Color.red;
        [SerializeField] private float colorTransitionDuration;
        
        [Header("Sound")]
        [SerializeField] private EventReference attackSound;
        [SerializeField] private float soundDistance = 2f;
        
        [Header("Warn Settings")]
        [SerializeField] private float warningDuration;
        [SerializeField] private SpriteRenderer rangeSprite;
        [SerializeField] private float rangeTransparencyOnWarn;
        [SerializeField] private float fadeDuration;

        private CancellationTokenSource _cancelAttackCts = new();
        private CancellationToken _destroyCancellationToken;

        private AudioManager _audioManager;
        private Color _initRangeColor;
        private PlayerHealth _playerHealth;
        private bool _isPlayerInRange;
        private bool _isWarning;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        private void OnEnable()
        {
            _isWarning = false;
            _isPlayerInRange = false;
        }
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

        private void Update()
        {
            if (_isPlayerInRange)
            {
                if (!_isWarning)
                {
                    WarnAsync(_cancelAttackCts.Token).Forget();
                }
            }
            else
            {
                if (_isWarning && aiPath.enabled)
                {
                    _isWarning = false;
                    CancelRecreateCts();
                    FadeArea(0f, fadeDuration, _destroyCancellationToken);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = false;
            }
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _isPlayerInRange = true;
            }
        }

        private async UniTask WarnAsync(CancellationToken token)
        {
            try
            {
                _isWarning = true;
                await FadeArea(rangeTransparencyOnWarn, fadeDuration, token);
                await UniTask.Delay(TimeSpan.FromSeconds(warningDuration), cancellationToken: token);
                _isWarning = false;
                await AttackAsync(_destroyCancellationToken);
            }
            catch (OperationCanceledException)
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
            aiPath.canMove = false;
            await ShowStartAttackAsync(token);
            if (_isPlayerInRange)
            {
                _playerHealth.TakeDamageWithKnockback(attackDamage, transform);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token);
            await ShowEndAttackAsync(token);
            aiPath.canMove = true;
        }

        private async UniTask ShowStartAttackAsync(CancellationToken token)
        {
            await rangeSprite.DOColor(attackColor, colorTransitionDuration).ToUniTask(cancellationToken: token);
            impulseSource.GenerateImpulse(impulseStrength);
            _audioManager.PlayOneShot(attackSound, gameObject.transform.position, 
                _playerHealth.transform.position, soundDistance);
            Instantiate(impactParticlesPrefab, transform);
        }

        private async UniTask ShowEndAttackAsync(CancellationToken token)
        {
            await rangeSprite.DOColor(_initRangeColor, colorTransitionDuration).ToUniTask(cancellationToken: token);
            await FadeArea(0f, fadeDuration, token);
        }
        private void CancelRecreateCts()
        {
            _cancelAttackCts?.Cancel();
            _cancelAttackCts?.Dispose();
            _cancelAttackCts = new CancellationTokenSource();
        }
    }
}