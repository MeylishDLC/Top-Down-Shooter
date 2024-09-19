using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Menus
{
    public class MainMenu: MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        private void Start()
        {
            playButton.onClick.AddListener(StartGame);
            exitButton.onClick.AddListener(ExitGame);
        }
        private void StartGame()
        {
            playButton.interactable = false;
            exitButton.interactable = false;
            settingsButton.interactable = false;
            
            SceneManager.LoadScene("SampleScene");
        }
        private void ExitGame()
        {
            Application.Quit();
        }
    }
}