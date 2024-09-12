using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        private Rigidbody2D _rb;
        private Vector2 _movement;
        private Animator _animator;
        
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
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            if (_movement != Vector2.zero)
            {
                _animator.SetFloat(horizontal, _movement.x);
                _animator.SetFloat(vertical, _movement.y);
            }
            _animator.SetFloat(speed, _movement.sqrMagnitude);
        }
        private void FixedUpdate()
        {
            if (_movement.magnitude > 1)
            {
                _movement.Normalize();
            }
            _rb.MovePosition(_rb.position + _movement * movementSpeed * Time.fixedDeltaTime);
        }
    }
}
