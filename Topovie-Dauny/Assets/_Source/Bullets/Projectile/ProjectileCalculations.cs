using UnityEngine;

namespace Bullets.Projectile
{
    public class ProjectileCalculations
    {
        public Vector3 ProjectileMoveDir { get; private set; }
        public float NextYTrajectoryPosition { get; private set; }
        public float NextPositionYCorrectionAbsolute { get; private set; }
        
        private float _moveSpeed;
        private float _maxMoveSpeed;
        private float _trajectoryMaxRelativeHeight;

        private AnimationCurve _trajectoryCurve;
        private AnimationCurve _axisCorrectionCurve;
        private AnimationCurve _speedCurve;

        private Vector3 _trajectoryStartPoint;
        private Vector3 _initialTargetPosition;
        
        private ProjectileVisual _projectileVisual;
        
        public void Initialize(Transform target, Transform projectile, ProjectileConfig config, ProjectileVisual projectileVisual)
        {
            _trajectoryStartPoint = projectile.transform.position;
            _projectileVisual = projectileVisual;
            SetAnimationCurves(config);
            SetProjectileParameters(target, projectile, config);
        }
        
        private void SetProjectileParameters(Transform target, Transform projectile, ProjectileConfig config)
        {
            _maxMoveSpeed = config.MaxMoveSpeed;
            _initialTargetPosition = target.position;
            var xDistanceToTarget = Mathf.Abs(_initialTargetPosition.x - projectile.position.x);
            _trajectoryMaxRelativeHeight = xDistanceToTarget * config.MaxHeight;
            
            _projectileVisual?.SetTarget(target);
        }
        private void SetAnimationCurves(ProjectileConfig config)
        {
            _trajectoryCurve = config.TrajectoryAnimationCurve;
            _axisCorrectionCurve = config.AxisCorrectionAnimationCurve;
            _speedCurve = config.ProjectileSpeedAnimationCurve;
        }
        
        public void UpdateProjectilePosition(Transform projectile)
        {
            var trajectoryRange = _initialTargetPosition - _trajectoryStartPoint;

            if (trajectoryRange.x < 0)
            {
                _moveSpeed = -_moveSpeed;
            }

            var nextPositionX = projectile.transform.position.x + _moveSpeed * Time.deltaTime;
            var nextPositionXNormalized = (nextPositionX - _trajectoryStartPoint.x) / trajectoryRange.x;

            nextPositionXNormalized = Mathf.Clamp01(nextPositionXNormalized);

            var nextPositionYNormalized = _trajectoryCurve.Evaluate(nextPositionXNormalized);
            NextYTrajectoryPosition = nextPositionYNormalized * _trajectoryMaxRelativeHeight;

            var nextPositionYCorrectionNormalized = _axisCorrectionCurve.Evaluate(nextPositionXNormalized);
            NextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;

            var nextPositionY = _trajectoryStartPoint.y + NextYTrajectoryPosition + NextPositionYCorrectionAbsolute;

            var newPosition = Vector3.Lerp(_trajectoryStartPoint, _initialTargetPosition, nextPositionXNormalized);
            newPosition.y = nextPositionY;

            CalculateNextProjectileSpeed(nextPositionXNormalized);

            ProjectileMoveDir = newPosition - projectile.transform.position;

            projectile.transform.position = newPosition;
        }
        private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
        {
            var nextMoveSpeedNormalized = _speedCurve.Evaluate(nextPositionXNormalized);
            _moveSpeed = nextMoveSpeedNormalized * _maxMoveSpeed;
        }
    }
}