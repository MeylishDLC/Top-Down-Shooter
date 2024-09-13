using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.PlayerMovement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator[] sides; 
        [SerializeField] private float dodgeSpeed = 15f; 
        [SerializeField] private int dodgeTimeMilliseconds = 500;
        
        private Rigidbody2D _rb;
        private float _vertical;
        private float _horizontal;
        private Vector2 _direction;
        private bool _dodgeRoll;
        
        private static readonly int isWalking = Animator.StringToHash("isWalking");
        private static readonly int isRolling = Animator.StringToHash("isRolling");

        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }
        private void Update()
        {
            if (_dodgeRoll)
            {
                _rb.AddForce(_direction * dodgeSpeed);
            }
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            if (_horizontal > 0 || _horizontal < 0 || _vertical < 0 || _vertical > 0)
            {
                foreach (var side in sides)
                {
                    side.SetBool(isWalking, true);
                }
            }
            else
            {
                foreach (var side in sides)
                {
                    side.SetBool(isWalking, false);
                }
            }
            
            _direction = new Vector2(_horizontal, _vertical);
            if (Input.GetMouseButtonDown(1) && !CheckRolling.IsRolling)
            {
                foreach (var side in sides)
                {
                    side.SetTrigger(isRolling);
                    
                    RollAsync(CancellationToken.None).Forget();
                }
            }

        }
        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_horizontal, _vertical);
        }

        private async UniTask RollAsync(CancellationToken token)
        {
            _dodgeRoll = true;
            await UniTask.Delay(dodgeTimeMilliseconds, cancellationToken: token);
            _dodgeRoll = false;
        }
        
    }
}
