using Enemies;
using UnityEngine;

namespace Weapons.AbilityWeapons
{
    public class KnifeObject: MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private int damageAmount;
        [SerializeField] private float lifetime;

        private Rigidbody2D _rb;
        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, lifetime);
        }

        public void ShootInDirection(Vector3 direction)
        {
            direction.Normalize();
            _rb.velocity = direction * speed;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
                enemyHealth.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}