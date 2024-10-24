using Enemies;
using UnityEngine;

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [Range(1, 10)] [SerializeField] private float speed;
        [SerializeField] private int damageAmount;
        [SerializeField] private float lifetime;

        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, lifetime);
        }

        private void FixedUpdate()
        {
            _rb.velocity = transform.right * speed;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
                enemyHealth.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}
