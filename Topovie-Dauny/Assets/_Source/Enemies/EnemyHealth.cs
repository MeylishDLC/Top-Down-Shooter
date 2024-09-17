using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemies.Combat;
using Pathfinding;
using Player.PlayerMovement;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyHealth: MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int maxMoneyDrop;
        
        [Header("Color Changing")]
        [SerializeField] private Color colorOnDamageTaken;
        [SerializeField] private int stayTimeMilliseconds;
        
        [Header("Knockback Settings")]
        [SerializeField] private int knockBackTimeMilliseconds = 200;
        [SerializeField] private float knockbackThrust = 15f;

        private Transform _playerTransform;
        private AIPath _aiPathComponent;
        private SpriteRenderer _spriteRenderer;
        private KnockBack _knockBack;
        private PlayerMovement _playerMovement;
        private int _currentHealth;
        private bool _isDead;

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            _playerTransform = playerMovement.gameObject.transform;
        }
        private void Start()
        {
            _currentHealth = maxHealth;
            _aiPathComponent = GetComponent<AIPath>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            var destinationSetter = GetComponent<AIDestinationSetter>();
            destinationSetter.target = _playerTransform;
            
            _knockBack = new(GetComponent<Rigidbody2D>(), knockBackTimeMilliseconds, knockbackThrust);
        }

        private void Update()
        {
            DisableMovementOnKnockback();
        }

        public void TakeDamage(int damage)
        {
            if (!_isDead)
            {
                _currentHealth -= damage;
                
                //todo: check this one?
                ChangeColor(CancellationToken.None).Forget();
                _knockBack.GetKnockedBack(_playerMovement.transform);
                
                CheckIfDeadAsync(CancellationToken.None).Forget();
            }
        }

        private void DisableMovementOnKnockback()
        {
            if (_knockBack.GettingKnockedBack && !_isDead)
            {
                _aiPathComponent.enabled = false;
            }

            if (!_knockBack.GettingKnockedBack && !_isDead && !_aiPathComponent.enabled)
            {
                _aiPathComponent.enabled = true;
            }
        }
        private async UniTask ChangeColor(CancellationToken token)
        {
            _spriteRenderer.color = colorOnDamageTaken;
            await UniTask.Delay(stayTimeMilliseconds, cancellationToken: token);
            _spriteRenderer.color = Color.white;
        }
        private async UniTask CheckIfDeadAsync(CancellationToken token)
        {
            if (_currentHealth <= 0)
            {
                _isDead = true;
                _aiPathComponent.enabled = false;
                await gameObject.transform.DOScaleX(0f, 0.5f).ToUniTask();
                
                Destroy(gameObject);
            }
        }
        
    }
}