using System;
using System.Threading;
using Bullets.Projectile;
using Cysharp.Threading.Tasks;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Bomb", menuName = "Combat/Abilities/Bomb")]
    public class Bomb: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;

        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float projectileMaxMoveSpeed;
        [SerializeField] private float projectileMaxHeight;
        [SerializeField] private ProjectileConfig config;
        
        private Transform _target;
        private Transform _playerTransform;
        private ProjectContext _projectContext;
        private Vector3 _targetPosition;
        public override void Construct(PlayerMovement playerMovement, ProjectContext projectContext)
        {
            _playerTransform = playerMovement.transform;
            _projectContext = projectContext;
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            SetTarget();
            ThrowBomb();
            OnAbilitySuccessfullyUsed?.Invoke();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token); 
        }
        private void SetTarget()
        {
            var cursorScreenPosition = Input.mousePosition;

            var cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition);

            _targetPosition = new Vector3(cursorWorldPosition.x, cursorWorldPosition.y, 0);

            _target = new GameObject("BombTarget").transform;
            _target.position = _targetPosition;
        }
        private void ThrowBomb()
        {
            var projectile = _projectContext.Container.InstantiatePrefabForComponent<Projectile>(bombPrefab); 
            projectile.transform.position = _playerTransform.position;
            projectile.Initialize(_target, config);
            Destroy(_target.gameObject, projectile.Lifetime);
        }
    }
}