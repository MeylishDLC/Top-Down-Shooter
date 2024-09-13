using System.Collections;
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
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }
        void Update()
        {
            if (_dodgeRoll)
            {
                _rb.AddForce(_direction * dodgeSpeed);
            }
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            if (_horizontal > 0 || _horizontal < 0 || _vertical < 0 || _vertical > 0)
            {
                for (int i = 0; i < sides.Length; i++)
                {
                    sides[i].SetBool("Walking", true);
                }
            }
            else
            {
                for (int i = 0; i < sides.Length; i++)
                {
                    sides[i].SetBool("Walking", false);
                }
      
            }
            _direction = new Vector2(_horizontal, _vertical);
            if (Input.GetMouseButtonDown(1) && !CheckRolling.IsRolling)
            {
                for (int i = 0; i < sides.Length; i++)
                {
                    sides[i].SetTrigger("Rollin");

                    //StartCoroutine(Roll());
                    
                    RollAsync().Forget();
                }
            }

        }
        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_horizontal, _vertical);
        }

        private async UniTask RollAsync()
        {
            _dodgeRoll = true;
            await UniTask.Delay(dodgeTimeMilliseconds);
            _dodgeRoll = false;
        }
        
        // IEnumerator Roll()
        // {
        //     DodgeRoll = true;
        //     yield return new WaitForSeconds(DodgeTime);
        //     DodgeRoll = false;
        // }
    }
}
