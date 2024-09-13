using UnityEngine;

namespace Enemies
{
    public class EnemyMovement: MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 3f; // Speed of the enemy
        private Rigidbody2D _rb;
        private Animator _animator;

        [SerializeField] private Transform _player; // Reference to the player

        private static readonly int horizontal = Animator.StringToHash("horizontal");
        private static readonly int vertical = Animator.StringToHash("vertical");
        private static readonly int speed = Animator.StringToHash("speed");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            
        }

        private void Update()
        {
            // Calculate the direction towards the player
            Vector2 direction = (_player.position - transform.position).normalized;

            // Set animator parameters based on direction
            if (direction != Vector2.zero)
            {
                _animator.SetFloat(horizontal, direction.x);
                _animator.SetFloat(vertical, direction.y);
            }
            _animator.SetFloat(speed, direction.sqrMagnitude);
        }

        private void FixedUpdate()
        {
            // Calculate the movement vector towards the player
            Vector2 direction = (_player.position - transform.position).normalized;

            // Move the enemy towards the player
            _rb.MovePosition(_rb.position + direction * movementSpeed * Time.fixedDeltaTime);
        }
    }
}