using System;
using UnityEngine;

namespace Player.PlayerCombat.Projectile
{
    public class Projectile: MonoBehaviour
    {
        [SerializeField] private float lifetime;
        
        private Transform _target;
        private float _moveSpeed;
        private float _maxMoveSpeed;
        private float _trajectoryMaxRelativeHeight;
        private float _distanceToTargetToDestroyProjectile = 0.05f;
        
        private AnimationCurve _trajectoryAnimationCurve;
        private AnimationCurve _axisCorrectionAnimationCurve;
        private AnimationCurve _projectileSpeedAnimationCurve;
        
        private Vector3 _trajectoryStartPoint;
        private void Start()
        {
            _trajectoryStartPoint = transform.position;
            Destroy(gameObject, lifetime);
        }
        private void Update()
        {
            UpdateProjectilePosition();
            
            if (Vector3.Distance(transform.position, _target.position) < _distanceToTargetToDestroyProjectile)
            {
                Destroy(gameObject);
            }
        }
        public void InitializeProjectile(Transform target, float maxMoveSpeed, float trajectoryMaxHeight)
        {
            _target = target;
            _maxMoveSpeed = maxMoveSpeed;

            var xDistanceToTarget = target.position.x - transform.position.x;
            _trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;
        }

        public void InitializeAnimationCurves(AnimationCurve trajectoryCurve, AnimationCurve axisCorrectionCurve, 
            AnimationCurve projectileSpeedCurve)
        {
            _trajectoryAnimationCurve = trajectoryCurve;
            _axisCorrectionAnimationCurve = axisCorrectionCurve;
            _projectileSpeedAnimationCurve = projectileSpeedCurve;
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
            
            var nextPositionYCorrectionNormalized = _axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
            var nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;
            
            var nextPositionY = _trajectoryStartPoint.y + nextPositionYNormalized * 
                _trajectoryMaxRelativeHeight + nextPositionYCorrectionAbsolute;

            var newPosition = Vector3.Lerp(_trajectoryStartPoint, _target.position, nextPositionXNormalized);
            newPosition.y = nextPositionY;

            CalculateNextProjectileSpeed(nextPositionXNormalized);
            transform.position = newPosition;
        }

        private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
        {
            var nextMoveSpeedNormalized = _projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized);
            _moveSpeed = nextMoveSpeedNormalized * _maxMoveSpeed;
        }
    }
}