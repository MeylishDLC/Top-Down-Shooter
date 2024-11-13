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
        
        [Header("Settings Screen")]
        [SerializeField] private RectTransform settingsScreen;
        [SerializeField] private Button returnButton;
        
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        private void Start()
        {
            settingsScreen.gameObject.SetActive(false);
            
            playButton.onClick.AddListener(LoadChooseLevelScreen);
            settingsButton.onClick.AddListener(OpenSettingsScreen);
            exitButton.onClick.AddListener(ExitGame);
            returnButton.onClick.AddListener(CloseSettingsScreen);
        }
        private void LoadChooseLevelScreen()
        {
            SetButtonsInteractable(false);
            //todo load scene with level choose
            _sceneLoader.LoadSceneAsync(1, CancellationToken.None).Forget();
        }
        private void ExitGame()
        {
            Application.Quit();
        }
        private void OpenSettingsScreen()
        {
            settingsScreen.gameObject.SetActive(true);
        }
        private void CloseSettingsScreen()
        {
            settingsScreen.gameObject.SetActive(false);
        }
        private void SetButtonsInteractable(bool isEnabled)
        {
            playButton.interactable = isEnabled;
            exitButton.interactable = isEnabled;
            settingsButton.interactable = isEnabled;
        }
    }
}