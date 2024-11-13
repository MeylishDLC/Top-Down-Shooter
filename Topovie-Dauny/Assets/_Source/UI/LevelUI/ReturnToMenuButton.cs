using System;
using System.Threading;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.LevelUI
{
    public class ReturnToMenuButton: MonoBehaviour
    {
        private Button _button;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ReturnToMenu);
        }
        private void ReturnToMenu()
        {
            _button.interactable = false;
            _sceneLoader.LoadSceneAsync(0).Forget();
        }
    }
}