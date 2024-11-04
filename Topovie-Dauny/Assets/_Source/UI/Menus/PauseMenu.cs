using System.Threading;
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
        [SerializeField] private GameObject pauseMenu; 
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject optionsScreen;
        [SerializeField] private CustomCursor customCursor;
        
        private SceneLoader _sceneLoader;
        public bool IsPaused { get; private set; }
        private bool _optionsMenuActive;

        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        private void Start()
        {
            exitButton.onClick.AddListener(ExitGame);
            optionsButton.onClick.AddListener(OpenOptionsPanel);
            restartButton.onClick.AddListener(RestartLevel);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
        }

        private void PauseGame()
        {
            Cursor.visible = true;
            customCursor.gameObject.SetActive(false);
            
            IsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        private void ResumeGame()
        {
            Cursor.visible = false;
            customCursor.gameObject.SetActive(true);
            
            IsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        private void RestartLevel()
        {
            RestartLevelAsync(CancellationToken.None).Forget();
        }
        private async UniTask RestartLevelAsync(CancellationToken token)
        {
            optionsButton.interactable = false;
            restartButton.interactable = false;
            exitButton.interactable = false;

            Time.timeScale = 1f;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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