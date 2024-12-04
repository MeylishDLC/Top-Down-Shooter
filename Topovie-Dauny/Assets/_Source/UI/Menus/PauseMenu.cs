using Core.InputSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Menus
{
    public class PauseMenu: MonoBehaviour
    {
        public bool IsPaused { get; private set; }

        [SerializeField] private GameObject pauseMenu; 
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button resumeButton;
        
        [Header("Options Screen")]
        [SerializeField] private GameObject optionsScreen;
        [SerializeField] private Button optionsReturnButton;
        
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
            resumeButton.onClick.AddListener(ResumeGame);
            optionsButton.onClick.AddListener(OpenOptionsPanel);
            menuButton.onClick.AddListener(GoToMainMenu);
            optionsReturnButton.onClick.AddListener(ReturnToPause);
            
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
        private void GoToMainMenu()
        {
            _inputListener.SetInput(false);
            Time.timeScale = 1f;
            
            _sceneLoader.LoadSceneAsync(0).Forget();
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
    }
}