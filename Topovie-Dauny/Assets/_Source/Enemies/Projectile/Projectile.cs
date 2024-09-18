using UnityEngine;

namespace Enemies.Projectile
{
    public class Projectile: MonoBehaviour
    {
        [SerializeField] private float lifetime;
        [SerializeField] private ProjectileVisual projectileVisual;
        
        private Transform _target;
        private float _moveSpeed;
        private float _maxMoveSpeed;
        private float _trajectoryMaxRelativeHeight;
        private float _distanceToTargetToDestroyProjectile = 0.01f;
        
        private AnimationCurve _trajectoryAnimationCurve;
        private AnimationCurve _axisCorrectionAnimationCurve;
        private AnimationCurve _projectileSpeedAnimationCurve;
        
        private Vector3 _trajectoryStartPoint;
        private Vector3 _projectileMoveDir;

        private float _nextYTrajectoryPosition;
        private float _nextPositionYCorrectionAbsolute;
        private void Start()
        {
            _trajectoryStartPoint = transform.position;
            Destroy(gameObject, lifetime);
        }
        private void Update()
        {
            UpdateProjectilePosition();
            
            // if (Vector3.Distance(transform.position, _target.position) < _distanceToTargetToDestroyProjectile)
            // {
            //     Destroy(gameObject);
            // }
        }
        public void InitializeProjectile(Transform target, float maxMoveSpeed, float trajectoryMaxHeight)
        {
            _target = target;
            _maxMoveSpeed = maxMoveSpeed;

            var xDistanceToTarget = target.position.x - transform.position.x;
            _trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;
            
            projectileVisual.SetTarget(target);
        }

        public void InitializeAnimationCurves(AnimationCurve trajectoryCurve, AnimationCurve axisCorrectionCurve, 
            AnimationCurve projectileSpeedCurve)
        {
            _trajectoryAnimationCurve = trajectoryCurve;
            _axisCorrectionAnimationCurve = axisCorrectionCurve;
            _projectileSpeedAnimationCurve = projectileSpeedCurve;
        }
        public Vector3 GetProjectileMoveDir()
        {
            return _projectileMoveDir;
        }
        public float GetNextYTrajectoryPosition()
        {
            return _nextYTrajectoryPosition;
        }
        public float GetNextPositionYCorrectionAbsolute()
        {
            return _nextPositionYCorrectionAbsolute;
        }
        private void UpdateProjectilePosition()
        {
            var trajectoryRange = _target.position - _trajectoryStartPoint;
            
            if (trajectoryRange.x < 0)
            {
                _moveSpeed = -_moveSpeed;
            }
            
            var nextPositionX = transform.position.x + _moveSpeed * Time.deltaTime;
            var nextPositionXNormalized = (nextPositionX - _trajectoryStartPoint.x) / trajectoryRange.x;

            nextPositionXNormalized = Mathf.Clamp01(nextPositionXNormalized);

            var nextPositionYNormalized = _trajectoryAnimationCurve.Evaluate(nextPositionXNormalized);
            _nextYTrajectoryPosition = nextPositionYNormalized * _trajectoryMaxRelativeHeight;
            
            var nextPositionYCorrectionNormalized = _axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
            _nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;
            
            var nextPositionY = _trajectoryStartPoint.y + _nextYTrajectoryPosition + _nextPositionYCorrectionAbsolute;

            var newPosition = Vector3.Lerp(_trajectoryStartPoint, _target.position, nextPositionXNormalized);
            newPosition.y = nextPositionY;

            CalculateNextProjectileSpeed(nextPositionXNormalized);

            _projectileMoveDir = newPosition - transform.position;
            
            transform.position = newPosition;
        }

        private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
        {
            var nextMoveSpeedNormalized = _projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized);
            _moveSpeed = nextMoveSpeedNormalized * _maxMoveSpeed;
        }

    }
}