using UnityEngine;

namespace Bullets.Projectile
{
    public class ProjectileVisual : MonoBehaviour
    {
        [SerializeField] private Transform projectileVisual;
        [SerializeField] private Transform projectileShadow;
        [SerializeField] private EnemyProjectile projectile;
        [SerializeField] private float rotationSpeed = 5f;

        private Transform _target;
        private Vector3 _trajectoryStartPosition;
        private float _shadowPositionYDivider = 6f;
        private void Start()
        {
            _trajectoryStartPosition = transform.position;
        }
        private void Update()
        {
            UpdateProjectileRotation();
            UpdateShadowPosition();

            var trajectoryProgressMagnitude = (transform.position - _trajectoryStartPosition).magnitude;
            var trajectoryMagnitude = (_target.position - _trajectoryStartPosition).magnitude;

            var trajectoryProgressNormalized = trajectoryProgressMagnitude / trajectoryMagnitude;
            if (trajectoryProgressNormalized < 0.6f)
            {
                UpdateProjectileShadowRotation();
            }
        }
        private void UpdateProjectileRotation()
        {
            var projectileMoveDir = projectile.ProjectileMoveDir;
    
            var targetAngle = Mathf.Atan2(projectileMoveDir.y, projectileMoveDir.x) * Mathf.Rad2Deg;
    
            var currentRotation = projectileVisual.transform.rotation;
    
            var targetRotation = Quaternion.Euler(0, 0, targetAngle);
    
            projectileVisual.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        } 
        private void UpdateProjectileShadowRotation()
        {
            var projectileMoveDir = projectile.ProjectileMoveDir;
    
            var targetAngle = Mathf.Atan2(projectileMoveDir.y, projectileMoveDir.x) * Mathf.Rad2Deg;
    
            var currentRotation = projectileShadow.transform.rotation;
    
            var targetRotation = Quaternion.Euler(0, 0, targetAngle);
    
            projectileShadow.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        private void UpdateShadowPosition()
        {
            var newPosition = transform.position;
            newPosition.y = _trajectoryStartPosition.y + projectile.NextYTrajectoryPosition / _shadowPositionYDivider +
                            projectile.NextPositionYCorrectionAbsolute;
            projectileShadow.position = newPosition;
        }
        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}
