using System.Threading;
using Cysharp.Threading.Tasks;
using DialogueSystem;
using Player.PlayerCombat;
using UnityEngine;
using Zenject;

namespace Player.PlayerMovement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator[] sides;
        [SerializeField] private float movementSpeed = 1f; 
        [SerializeField] private float dodgeSpeed = 15f; 
        [SerializeField] private int dodgeTimeMilliseconds = 500;
        [SerializeField] private PlayerHealth playerHealth;
        
        private Rigidbody2D _rb;
        private float _vertical;
        private float _horizontal;
        private Vector2 _direction;
        private bool _dodgeRoll;
        private DialogueManager _dialogueManager;
        
        private static readonly int isWalking = Animator.StringToHash("isWalking");
        private static readonly int isRolling = Animator.StringToHash("isRolling");

        [Inject]
        public void Construct(DialogueManager dialogueManager)
        {
            _dialogueManager = dialogueManager;
        }
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }
        private void Update()
        {
            if (!_dialogueManager.DialogueIsPlaying && !playerHealth.KnockBack.GettingKnockedBack)
            {
                if (_dodgeRoll)
                {
                    _rb.AddForce(_direction * dodgeSpeed);
                }
                HandleMovement(); 
                HandleRolling();
            }
        }
        private void FixedUpdate()
        {
            if (!_dialogueManager.DialogueIsPlaying && !playerHealth.KnockBack.GettingKnockedBack)
            {
                _rb.velocity = new Vector2(_horizontal, _vertical).normalized * movementSpeed;
            }
        }

        private void HandleMovement()
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            if (_horizontal > 0 || _horizontal < 0 || _vertical < 0 || _vertical > 0)
            {
                foreach (var side in sides)
                {
                    if (side.gameObject.activeSelf)
                    {
                        side.SetBool(isWalking, true);
                    }
                }
            }
            else
            {
                foreach (var side in sides)
                {
                    if (side.gameObject.activeSelf)
                    {
                        side.SetBool(isWalking, false);
                    }
                }
            }
            
            _direction = new Vector2(_horizontal, _vertical);
        }

        private void HandleRolling()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !CheckRolling.IsRolling)
            {
                foreach (var side in sides)
                {
                    if (side.gameObject.activeSelf)
                    {
                        side.SetTrigger(isRolling);
                    }
                    
                    RollAsync(CancellationToken.None).Forget();
                }
            }
        }

        private async UniTask RollAsync(CancellationToken token)
        {
            _dodgeRoll = true;
            await UniTask.Delay(dodgeTimeMilliseconds, cancellationToken: token);
            _dodgeRoll = false;
        }
        
    }
}
