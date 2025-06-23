using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player.PlayerCombat
{
    public class PlayerView
    {
        private static readonly int isWalking = Animator.StringToHash("isWalking");
        private static readonly int isRolling = Animator.StringToHash("isRolling");
        private static readonly int isDead = Animator.StringToHash("isDead");
        
        private Animator[] _sides;
        
        public PlayerView(Animator[] sides)
        {
            _sides = sides;
        }
        public void HandleWalkingAnimation(bool isMoving)
        {
            foreach (var side in _sides)
            {
                if (side.gameObject.activeSelf)
                {
                    side.SetBool(isWalking, isMoving);
                }
            }
        }
        public void PlayRollAnimation()
        {
            foreach (var side in _sides)
            {
                if (side.gameObject.activeSelf)
                {
                    side.SetTrigger(isRolling);
                }
            }
        }
        public void PlayDeathAnimation()
        {
            foreach (var side in _sides)
            {
                if (side.gameObject.activeSelf)
                {
                    side.SetTrigger(isDead);
                }
            }
        }
    }
}