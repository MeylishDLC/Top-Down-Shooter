using System;
using System.Threading;
using Core.InputSystem;
using Core.LevelSettings;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UI.LevelUI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameEnvironment.LevelObjects
{
    public class Portal: MonoBehaviour
    {
        [SerializeField] private ConfirmationScreen confirmationScreen;
        [SerializeField] private Light2D portalLight;
        [SerializeField] private Collider2D rangeTrigger;
        [SerializeField] private SpriteRenderer textBubble;
        
        private StatesChanger _statesChanger;
        private SceneLoader _sceneLoader;
        private InputListener _inputListener;
        private LevelSave _levelSave;
        private bool _isInRange;
        
        [Inject]
        public void Construct(StatesChanger statesChanger, SceneLoader sceneLoader, 
            InputListener inputListener, LevelSave levelSave)
        {
            _levelSave = levelSave;
            _inputListener = inputListener;
            _sceneLoader = sceneLoader;
            _statesChanger = statesChanger;
            
            _statesChanger.OnStateChanged += ActivateOnChangeState;
            confirmationScreen.OnConfirmed += GoToNextLevel;
            _inputListener.OnInteractPressed += ShowConfirmationScreen;
        }
        private void OnDestroy()
        {
            _statesChanger.OnStateChanged -= ActivateOnChangeState;
            confirmationScreen.OnConfirmed -= GoToNextLevel;
            _inputListener.OnInteractPressed -= ShowConfirmationScreen;
        }
        private void Awake()
        {
            DeactivatePortal();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                ShowTextBubble(true);
                _isInRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                ShowTextBubble(false);
                _isInRange = false;
            }
        }
        private void ActivateOnChangeState(GameStates state)
        {
            if (state == GameStates.PortalCharged)
            {
                ActivatePortal();
            }
        }
        private void ActivatePortal()
        {
            portalLight.gameObject.SetActive(true);
            rangeTrigger.enabled = true;
        }
        private void DeactivatePortal()
        {
            rangeTrigger.enabled = false;
            textBubble.gameObject.SetActive(false);
            portalLight.gameObject.SetActive(false);
        }
        private void ShowTextBubble(bool show)
        {
            textBubble.gameObject.SetActive(show);
        }
        private void ShowConfirmationScreen()
        {
            if (_isInRange)
            {
                confirmationScreen.OpenConfirmationScreen();
            }
        }
        private void GoToNextLevel()
        {
            var currentLevelNumber = SceneManager.GetActiveScene().buildIndex - 2;

            if (_levelSave.GetLevelsPassed() < currentLevelNumber + 1)
            {
                _levelSave.SaveProgress(SceneManager.GetActiveScene().buildIndex - 2);
            }
            
            _inputListener.SetInput(false);
            confirmationScreen.gameObject.SetActive(false);
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (SceneManager.sceneCountInBuildSettings <= nextSceneIndex)
            {
                _sceneLoader.LoadSceneAsync(0).Forget();
            }
            else
            {
                _sceneLoader.LoadSceneAsync(nextSceneIndex, false).Forget();
            }
        }
    }
}