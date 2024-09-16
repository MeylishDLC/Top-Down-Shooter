using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu: MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button exitButton;

        private bool _pauseMenuActive;
        private void Start()
        {
            exitButton.onClick.AddListener(ExitGame);
            resumeButton.onClick.AddListener(ExitGame);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_pauseMenuActive)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
        private void PauseGame()
        {
            _pauseMenuActive = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        private void ResumeGame()
        {
            _pauseMenuActive = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        private void ExitGame()
        {
            Application.Quit();
        }
    }
}