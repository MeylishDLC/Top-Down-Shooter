using System;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Menus
{
    public class TestSceneButton: MonoBehaviour
    {
        [SerializeField] private int testSceneIndex;
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
            _button.onClick.AddListener(OpenTestScene);
        }
        private void OpenTestScene()
        {
            _button.interactable = false;
            _sceneLoader.LoadSceneAsync(testSceneIndex, false).Forget();
        }
    }
}