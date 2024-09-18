using System;
using UnityEngine;

namespace Player.PlayerCombat.Projectile
{
    public class Projectile: MonoBehaviour
    {
        private Transform _target;
        private float _moveSpeed;
        private float _distanceToTargetToDestroyProjectile = 0.2f;

        private void Update()
        {
            
            if (_target == null)
            {
                Debug.LogWarning("Target is null. Destroying projectile.");
                Destroy(gameObject);
                return;
            }

            var moveDirNormalized = (_target.position - transform.position).normalized;

            transform.position += moveDirNormalized * _moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, _target.position) < _distanceToTargetToDestroyProjectile)
            {
                Destroy(gameObject);
            }
        }

        public void InitializeProjectile(Transform target, float moveSpeed)
        {
            _target = target;
            _moveSpeed = moveSpeed;
        }
    }
}