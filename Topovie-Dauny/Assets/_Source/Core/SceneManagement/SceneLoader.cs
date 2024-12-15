using System;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using SoundSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.SceneManagement
{
    public class SceneLoader: MonoBehaviour
    {
        public int LastSceneIndex {get; private set;}
        public int CurrentSceneIndex {get; private set;}
        private RectTransform _loadingScreen;
        private Slider _loadingSlider;
        private AudioManager _audioManager;
        public void Construct(RectTransform loadingScreen, Slider loadingSlider, AudioManager audioManager)
        {
            _loadingScreen = loadingScreen;
            _loadingSlider = loadingSlider;
            _audioManager = audioManager;
        }
        public async UniTask LoadSceneAsync(int index, bool disableScreenOnLoad = true)
        {
            LastSceneIndex = SceneManager.GetActiveScene().buildIndex; 
            CurrentSceneIndex = index;
            _loadingSlider.value = 0;
            _loadingScreen.gameObject.SetActive(true);
            _audioManager.StopPlayingMusic(STOP_MODE.IMMEDIATE);
            
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
        }
        public void SetLoadingScreenActive(bool active)
        {
            _loadingScreen.gameObject.SetActive(active);
        }
    }
}