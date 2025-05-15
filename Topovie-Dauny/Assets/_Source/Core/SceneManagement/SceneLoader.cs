using System;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using SoundSystem;
using TMPro;
using UI.Core.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Core.SceneManagement
{
    public class SceneLoader: MonoBehaviour
    {
        public int LastSceneIndex {get; private set;}
        public int CurrentSceneIndex {get; private set;}
        
        private RectTransform _loadingScreen;
        private Slider _loadingSlider;
        private TMP_Text _tipText;
        
        private AudioManager _audioManager;
        private LoadingTips _loadingTips;
        
        [Inject]
        public void Construct(LoadingScreenComponents components, AudioManager audioManager, LoadingTips loadingTips)
        {
            _loadingScreen = components.LoadingScreen;
            _loadingSlider = components.LoadingSlider;
            _tipText = components.TipText;
            
            _audioManager = audioManager;
            _loadingTips = loadingTips;
        }
        public async UniTask LoadSceneAsync(int index, bool disableScreenOnLoad = true)
        {
            LastSceneIndex = SceneManager.GetActiveScene().buildIndex; 
            CurrentSceneIndex = index;
            _loadingSlider.value = 0;
            _loadingScreen.gameObject.SetActive(true);
            
            _audioManager.StopPlayingMusic(STOP_MODE.IMMEDIATE);
            ShowTip();
            
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
        private void ShowTip()
        {
            _loadingTips.DisplayRandomTip(_tipText);
        }
    }
}