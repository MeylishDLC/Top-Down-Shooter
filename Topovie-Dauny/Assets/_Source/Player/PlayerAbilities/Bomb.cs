using System.Threading;
using Cysharp.Threading.Tasks;
using Enemies.Projectile;
using UnityEngine;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Bomb: Ability
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private Camera cinemachineCamera;
        
        [SerializeField] private float projectileMaxMoveSpeed;
        [SerializeField] private float projectileMaxHeight;
        
        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve trajectoryAnimationCurve;
        [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
        [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;
        
        private Transform _target;
        private Transform _playerTransform;
        private Vector3 _targetPosition;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerTransform = playerMovement.transform;
            AbilityType = AbilityTypes.HoldButton;
        }
        public override void UseAbility()
        {
            if (CanUse)
            {
                UseAbilityAsync(CancellationToken.None).Forget();
            }
        }

        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            CanUse = false;
            
            SetTarget();
            ThrowBomb();
            
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
            CanUse = true;
        }
        
        private void SetTarget()
        {
            var cursorScreenPosition = Input.mousePosition;

            var cursorWorldPosition = cinemachineCamera.ScreenToWorldPoint(cursorScreenPosition);

            _targetPosition = new Vector3(cursorWorldPosition.x, cursorWorldPosition.y, 0);

            _target = new GameObject("BombTarget").transform;
            _target.position = _targetPosition;
        }

        private void ThrowBomb()
        {
            var projectile = Instantiate(bombPrefab, _playerTransform.position, Quaternion.identity)
                .GetComponent<Projectile>(); 
            projectile.InitializeAll(_target,projectileMaxMoveSpeed, projectileMaxHeight, trajectoryAnimationCurve, 
                axisCorrectionAnimationCurve, projectileSpeedAnimationCurve);
            
            Destroy(_target.gameObject, projectile.lifetime);
        }
    }
}