using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Core.SceneManagement
{
    public class SceneLoader: MonoBehaviour
    {
        private RectTransform _loadingScreen;
        private Slider _loadingSlider;
        public int LastSceneIndex {get; private set;}
        public int CurrentSceneIndex {get; private set;}
        public void Construct(RectTransform loadingScreen, Slider loadingSlider)
        {
            _loadingScreen = loadingScreen;
            _loadingSlider = loadingSlider;
        }
        public async UniTask LoadSceneAsync(int index, bool disableScreenOnLoad = true)
        {
            LastSceneIndex = SceneManager.GetActiveScene().buildIndex; 
            _loadingSlider.value = 0;
            _loadingScreen.gameObject.SetActive(true);
            
            var asyncOperation = SceneManager.LoadSceneAsync(index);
            if (asyncOperation is null)
            {
                throw new Exception("Scene not found");
            }
            asyncOperation.allowSceneActivation = false;
            float progress = 0;
            while (!asyncOperation.isDone)
            {
                progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
                _loadingSlider.value = progress;
                if (progress >= 0.9f)
                {
                    _loadingSlider.value = 1;
                    asyncOperation.allowSceneActivation = true;
                }
                await UniTask.Yield();
            } 
            if (disableScreenOnLoad)
            {
                _loadingScreen.gameObject.SetActive(false);
            }
            CurrentSceneIndex = index;
        }
        public void SetLoadingScreenActive(bool active)
        {
            _loadingScreen.gameObject.SetActive(active);
        }
    }
}