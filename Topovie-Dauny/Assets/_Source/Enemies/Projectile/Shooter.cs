using UnityEngine;
using Zenject;

namespace Enemies.Projectile
{
    public class Shooter: MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float shootRate;
        [SerializeField] private float projectileMaxMoveSpeed;
        [SerializeField] private float projectileMaxHeight;
        
        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve trajectoryAnimationCurve;
        [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
        [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;
        
        private float _shootTimer;
        private Transform _target;
        public void Construct(Player.PlayerMovement.PlayerMovement playerMovement)
        {
            _target = playerMovement.transform;
        }
        private void Update()
        {
            _shootTimer -= Time.deltaTime;
            if (_shootTimer <= 0)
            {
                _shootTimer = shootRate;
                var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity)
                    .GetComponent<Projectile>();
                projectile.InitializeProjectile(_target,projectileMaxMoveSpeed, projectileMaxHeight);
                projectile.InitializeAnimationCurves(trajectoryAnimationCurve, axisCorrectionAnimationCurve, projectileSpeedAnimationCurve);
            }
        }
    }
}