﻿using System;
using UnityEngine;

namespace Bullets.Projectile
{
    public class ProjectileCalculations
    {
        public event Action OnDestinationReached;
        private float _nextYTrajectoryPosition;
        private float _nextPositionYCorrectionAbsolute;
        
        private float _moveSpeed;
        private float _maxMoveSpeed;
        private float _trajectoryMaxRelativeHeight;

        private AnimationCurve _trajectoryCurve;
        private AnimationCurve _axisCorrectionCurve;
        private AnimationCurve _speedCurve;

        private Vector3 _trajectoryStartPoint;
        private Vector3 _initialTargetPosition;
        
        public void Initialize(Transform target, Transform projectile, ProjectileConfig config)
        {
            _trajectoryStartPoint = projectile.transform.position;
            SetAnimationCurves(config);
            SetProjectileParameters(target, projectile, config);
        }
        private void SetProjectileParameters(Transform target, Transform projectile, ProjectileConfig config)
        {
            _maxMoveSpeed = config.MaxMoveSpeed;
            _initialTargetPosition = target.position;
            var xDistanceToTarget = Mathf.Abs(_initialTargetPosition.x - projectile.position.x);
            _trajectoryMaxRelativeHeight = xDistanceToTarget * config.MaxHeight;
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
            _nextYTrajectoryPosition = nextPositionYNormalized * _trajectoryMaxRelativeHeight;

            var nextPositionYCorrectionNormalized = _axisCorrectionCurve.Evaluate(nextPositionXNormalized);
            _nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;

            var nextPositionY = _trajectoryStartPoint.y + _nextYTrajectoryPosition + _nextPositionYCorrectionAbsolute;

            var newPosition = Vector3.Lerp(_trajectoryStartPoint, _initialTargetPosition, nextPositionXNormalized);
            newPosition.y = nextPositionY;

            CalculateNextProjectileSpeed(nextPositionXNormalized);
            projectile.transform.position = newPosition;
            CheckIfDistanceReached(newPosition);
        }
        private void CheckIfDistanceReached(Vector3 newPosition)
        {
            var roundedNewPos = new Vector3((float)Math.Round(newPosition.x), (float)Math.Round(newPosition.y), 
                (float)Math.Round(newPosition.z));
            var roundedInitPos = new Vector3((float)Math.Round(_initialTargetPosition.x), 
                (float)Math.Round(_initialTargetPosition.y), (float)Math.Round(_initialTargetPosition.z));
            
            if (roundedNewPos == roundedInitPos)
            {
                OnDestinationReached?.Invoke();
            }
        }
        private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
        {
            var nextMoveSpeedNormalized = _speedCurve.Evaluate(nextPositionXNormalized);
            _moveSpeed = nextMoveSpeedNormalized * _maxMoveSpeed;
        }
    }
}