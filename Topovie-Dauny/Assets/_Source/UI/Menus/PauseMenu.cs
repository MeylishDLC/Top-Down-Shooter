using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class PauseMenu: MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu; 
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject optionsScreen;
        public bool IsPaused { get; private set; }
        private bool _optionsMenuActive;
        private void Start()
        {
            exitButton.onClick.AddListener(ExitGame);
            optionsButton.onClick.AddListener(OpenOptionsPanel);
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
        public void PauseGame()
        {
            IsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        public void ResumeGame()
        {
            IsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
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