using System;
using System.Threading;
using Core.InputSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI.Menus
{
    public class PauseMenu: MonoBehaviour
    {
        public bool IsPaused { get; private set; }

        [SerializeField] private GameObject pauseMenu; 
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject optionsScreen;
        
        private CustomCursor _customCursor;
        private SceneLoader _sceneLoader;
        private InputListener _inputListener;
        private bool _optionsMenuActive;

        [Inject]
        public void Construct(SceneLoader sceneLoader, CustomCursor customCursor, InputListener inputListener)
        {
            _inputListener = inputListener;
            _customCursor = customCursor;
            _sceneLoader = sceneLoader;
        }
        private void Start()
        {
            exitButton.onClick.AddListener(ExitGame);
            optionsButton.onClick.AddListener(OpenOptionsPanel);
            restartButton.onClick.AddListener(RestartLevel);
            _inputListener.OnPausePressed += HandlePausePressed;
        }
        private void OnDestroy()
        {
            _inputListener.OnPausePressed -= HandlePausePressed;
        }
        private void HandlePausePressed()
        {
            if (IsPaused)
            {
                if (_optionsMenuActive)
                {
                    ReturnToPause();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
        private void PauseGame()
        {
            Cursor.visible = true;
            _customCursor.gameObject.SetActive(false);
            
            IsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        private void ResumeGame()
        {
            Cursor.visible = false;
            _customCursor.gameObject.SetActive(true);
            
            IsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        private void RestartLevel()
        {
            _inputListener.SetInput(false);
            RestartLevelAsync(CancellationToken.None).Forget();
        }
        private async UniTask RestartLevelAsync(CancellationToken token)
        {
            optionsButton.interactable = false;
            restartButton.interactable = false;
            exitButton.interactable = false;

            Time.timeScale = 1f;
            await _sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, token);
        }
        private void OpenOptionsPanel()
        {
            _optionsMenuActive = true;
            optionsScreen.SetActive(true);
            pauseMenu.SetActive(false);
        }
        private void ReturnToPause()
        {
            _optionsMenuActive = false;
            optionsScreen.SetActive(false);
            pauseMenu.SetActive(true);
        }
        private void ExitGame()
        {
            Debug.Log("Quitting game");
            Application.Quit();
        }
    }
}