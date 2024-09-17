using System;
using UnityEngine;

namespace Core
{
    public class RangeDetector: MonoBehaviour
    {
        public event Action OnPlayerEnterRange;
        public event Action OnPlayerExitRange;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerEnterRange?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerExitRange?.Invoke();
            }
        }
    }
}