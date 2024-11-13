using System;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Core.SceneManagement
{
    public class SceneLoader: MonoBehaviour
    {
        [SerializeField] private RectTransform loadingScreen;
        [SerializeField] private Slider loadingSlider;
        public async UniTask LoadSceneAsync(int index)
        {
            loadingSlider.value = 0;
            loadingScreen.gameObject.SetActive(true);
            
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
                loadingSlider.value = progress;
                if (progress >= 0.9f)
                {
                    loadingSlider.value = 1;
                    asyncOperation.allowSceneActivation = true;
                }
                await UniTask.Yield();
            }
        }

        public void SetLoadingScreenActive(bool active)
        {
            loadingScreen.gameObject.SetActive(active);
        }
    }
}