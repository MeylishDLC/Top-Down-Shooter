using UnityEngine;

namespace Bullets
{
    public class OrbitingEnemyBullet: EnemyBullet
    {
        private Vector3 _spawnerPosition;
        private float _angularSpeed;
        
        public void InitOrbitingBullet(Vector3 spawnerPosition, float angularSpeed)
        {
            _spawnerPosition = spawnerPosition;
            _angularSpeed = angularSpeed;
        }

        protected override void FixedUpdate()
        {
            Rb.velocity = transform.right * speed;
            transform.RotateAround(_spawnerPosition, Vector3.forward, _angularSpeed * Time.deltaTime);
        }
    }
}