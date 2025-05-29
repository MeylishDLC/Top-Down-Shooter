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
using UnityEngine.Serialization;
using Zenject;

namespace Enemies.EnemyTypes.Bug
{
    public class AreaAttacker: MonoBehaviour
    {
        public event Action OnWarnStarted;
        
        [SerializeField] private AIPath aiPath;
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private ParticleSystem impactParticlesPrefab;
        
        [Header("Material Settings")]
        [SerializeField] private SpriteRenderer rangeSprite;
        
        [Header("Attack Settings")]
        [SerializeField] private float impulseStrength;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackDuration;
        
        [Header("Warn Settings")]
        [SerializeField] private float warningDuration;
        
        [Header("Sound")]
        [SerializeField] private EventReference attackSound;
        [SerializeField] private float soundDistance = 2f;

        private CancellationTokenSource _cancelAttackCts = new();
        private CancellationToken _destroyCancellationToken;
        
        private BugAreaFader _bugAreaFader;
        private AudioManager _audioManager;
        private PlayerHealth _playerHealth;
        private bool _isPlayerInRange;
        private bool _isWarning;
        private bool _canDetect = true;

        [Inject]
        public void Construct(AudioManager audioManager, BugAreaFaderFactory faderFactory, BugAreaFaderConfig config)
        {
            _audioManager = audioManager;
            _bugAreaFader = faderFactory.Create(config);
        }
        private void OnEnable()
        {
            _isWarning = false;
            _isPlayerInRange = false;
            _canDetect = true;
        }
        private void Start()
        {
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            _playerHealth = enemyHealth.PlayerMovement.GetComponent<PlayerHealth>();
            
            _bugAreaFader.SetupFader(rangeSprite);
            _bugAreaFader.FadeAreaAsync(FadeType.FadeOut, _destroyCancellationToken).Forget();
            enemyHealth.OnEnemyDied += DisableDetectArea;
        }
        private void OnDestroy()
        {
            _cancelAttackCts?.Cancel();
            _cancelAttackCts?.Dispose();
            enemyHealth.OnEnemyDied += DisableDetectArea;
        }

        private void Update()
        {
            if (!_canDetect)
            {
                return;
            }
            if (_isPlayerInRange)
            {
                //area fade in if not already warning
                if (!_isWarning)
                {
                    WarnAsync(_cancelAttackCts.Token).Forget();
                }
            }
            else
            {
                //area fade out 
                if (_isWarning && aiPath.enabled)
                {
                    _isWarning = false;
                    CancelRecreateCts();
                    _bugAreaFader.FadeAreaAsync(FadeType.FadeOut, _destroyCancellationToken).Forget();
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
                OnWarnStarted?.Invoke();
                _isWarning = true;
                await _bugAreaFader.FadeAreaAsync(FadeType.FadeIn, token);
                await UniTask.Delay(TimeSpan.FromSeconds(warningDuration), cancellationToken: token);
                _isWarning = false;
                await AttackAsync(_destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
        
        private async UniTask AttackAsync(CancellationToken token)
        {
            aiPath.canMove = false;
            await ShowStartAttackAsync(token);
            if (_isPlayerInRange)
            {
                _playerHealth.TakeDamage(attackDamage);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: token);
            await ShowEndAttackAsync(token);
            aiPath.canMove = true;
        }

        private async UniTask ShowStartAttackAsync(CancellationToken token)
        {
            //color area into attack col
            await _bugAreaFader.ChangeAreaColorAsync(ColorChangeType.ChangeToAttackColor, token);
            impulseSource.GenerateImpulse(impulseStrength);
            _audioManager.PlayOneShot(attackSound, gameObject.transform.position, 
                _playerHealth.transform.position, soundDistance);
            
            Instantiate(impactParticlesPrefab, transform);
            _audioManager.PlayOneShot(_audioManager.FMODEvents.BugBlowUpSound, gameObject.transform.position,
                _playerHealth.transform.position, soundDistance);
        }

        private async UniTask ShowEndAttackAsync(CancellationToken token)
        {
            await _bugAreaFader.ChangeAreaColorAsync(ColorChangeType.ChangeToBaseColor, token);
            await _bugAreaFader.FadeAreaAsync(FadeType.FadeOut, token);
        }
        private void CancelRecreateCts()
        {
            _cancelAttackCts?.Cancel();
            _cancelAttackCts?.Dispose();
            _cancelAttackCts = new CancellationTokenSource();
        }
        private void DisableDetectArea()
        {
            _canDetect = false;
        }
    }
}