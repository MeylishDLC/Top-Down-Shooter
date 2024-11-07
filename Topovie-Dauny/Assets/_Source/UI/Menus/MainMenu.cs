using System;
using System.Threading;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI.Menus
{
    public class MainMenu: MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
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
            
            //todo change??
            _sceneLoader.LoadSceneAsync(1, CancellationToken.None).Forget();
        }
        private void ExitGame()
        {
            Application.Quit();
        }
    }
}