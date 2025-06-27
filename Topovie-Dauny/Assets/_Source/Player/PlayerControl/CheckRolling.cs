using System;
using UnityEngine;

namespace Player.PlayerControl
{
    public class CheckRolling: MonoBehaviour
    {
        public static bool IsRolling;
        private void OnDestroy()
        {
            IsRolling = false;
        }
        public void Roll()
        {
            IsRolling = true;
        }
        public void RollEnd()
        {
            IsRolling = false;
        }
    }
}