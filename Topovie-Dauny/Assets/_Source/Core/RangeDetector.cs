using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public class RangeDetector: MonoBehaviour
    {
        public event Action OnPlayerEnterRange;
        public event Action OnPlayerExitRange;

        private LevelSetter _levelSetter;
        
        [Inject]
        public void Construct(LevelSetter levelSetter)
        {
            _levelSetter = levelSetter;
        }
        private void Awake()
        {
            _levelSetter.OnStateChanged += EnableOnChangeState;
            EnableOnChangeState(States.Chill);
        }
        private void OnDestroy()
        {
            _levelSetter.OnStateChanged -= EnableOnChangeState;
        }
        private void EnableOnChangeState(States state)
        {
            enabled = state == States.Fight;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnPlayerEnterRange?.Invoke();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnPlayerExitRange?.Invoke();
            }
        }
    }
}