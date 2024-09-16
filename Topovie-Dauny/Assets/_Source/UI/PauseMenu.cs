using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu: MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [FormerlySerializedAs("resumeButton")] [SerializeField] private Button optionsButton;
        [SerializeField] private Button exitButton;

        private bool _pauseMenuActive;
        private void Start()
        {
            exitButton.onClick.AddListener(ExitGame);
            optionsButton.onClick.AddListener(OpenOptionsPanel);
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

        private void OpenOptionsPanel()
        {
            
        }
        private void ExitGame()
        {
            Debug.Log("Quitting game");
            Application.Quit();
        }
    }
}