using UnityEngine;
using Zenject;

namespace Player.PlayerCombat.Projectile
{
    public class Shooter: MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float shootRate;
        [SerializeField] private float projectileMoveSpeed;
        [SerializeField] private AnimationCurve trajectoryAnimationCurve;
        
        private float _shootTimer;
        private Transform _target;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
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
                projectile.InitializeProjectile(_target,projectileMoveSpeed);
            }
        }
    }
}