using UnityEngine;

namespace Player.PlayerMovement
{
    public class CheckRolling: MonoBehaviour
    {
        public static bool IsRolling;
        public void Roll()
        {
            IsRolling = true;
        }
        public void RollEnd()
        {
            IsRolling = false;
            transform.Rotate(0, 0, 0);
        }

    }
}