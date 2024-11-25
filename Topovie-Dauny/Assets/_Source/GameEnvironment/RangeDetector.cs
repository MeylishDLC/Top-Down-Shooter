using System;
using Core.LevelSettings;
using UnityEngine;
using Zenject;

namespace GameEnvironment
{
    public class RangeDetector: MonoBehaviour
    {
        public event Action OnPlayerEnterRange;
        public event Action OnPlayerExitRange;

        private StatesChanger _statesChanger;
        
        [Inject]
        public void Construct(StatesChanger statesChanger)
        {
            _statesChanger = statesChanger;
        }
        private void Awake()
        {
            _statesChanger.OnStateChanged += EnableOnChangeState;
            EnableOnChangeState(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= EnableOnChangeState;
        }
        private void EnableOnChangeState(GameStates gameState)
        {
            if (gameState == GameStates.Fight)
            {
                enabled = true;
            }
            else
            {
                enabled = false;
            }
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