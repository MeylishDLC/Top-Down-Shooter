using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu: MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu; 
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button exitButton;
        public bool IsPaused { get; private set; }
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
                    ResumeGame();
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
            
        }
        private void ExitGame()
        {
            Debug.Log("Quitting game");
            Application.Quit();
        }
    }
}