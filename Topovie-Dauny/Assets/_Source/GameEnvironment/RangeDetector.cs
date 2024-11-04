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

        private LevelChargesHandler _levelChargesHandler;
        
        [Inject]
        public void Construct(LevelChargesHandler levelChargesHandler)
        {
            _levelChargesHandler = levelChargesHandler;
        }
        private void Awake()
        {
            _levelChargesHandler.OnStateChanged += EnableOnChangeState;
            EnableOnChangeState(GameStates.Chill);
        }
        private void OnDestroy()
        {
            _levelChargesHandler.OnStateChanged -= EnableOnChangeState;
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
                Debug.Log("Is in range");
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnPlayerExitRange?.Invoke();
                Debug.Log("Is not in range");
            }
        }
    }
}