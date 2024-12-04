using System;
using System.Collections.Generic;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Cysharp.Threading.Tasks;
using Enemies.Boss;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Comics
{
    public class FinalComics: MonoBehaviour
    {
        [SerializeField] private BossLeo bossLeo;
        [SerializeField] private Image[] pages;
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private float delayBeforeComicsStart = 2f;
        [SerializeField] private float backgroundFadeInDuration = 0.25f;
        [SerializeField] private float comicsFadeInDuration = 0.25f;
        [SerializeField] private float pageFadeDuration = 0.25f;

        private Image _buttonImage;
        private int _currentPage;
        private CancellationToken _destroyCancellationToken;
        
        private void OnValidate()
        {
            if (TryGetComponent<Image>(out var img))
            {
                background = img;
            }
        }
        private void Awake()
        {
            bossLeo.OnBossDefeated += ShowComics;
        }
        private void Start()
        {
            button.interactable = false;
            _buttonImage = button.GetComponent<Image>();
            _destroyCancellationToken = this.GetCancellationTokenOnDestroy();
            FadeOutAll(0,_destroyCancellationToken).Forget();
            foreach (var page in pages)
            {
                page.gameObject.SetActive(false);
            }
            button.onClick.AddListener(GoToNextPage);
        }
        public void ShowComics()
        {
            FadeInAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask FadeInAsync(CancellationToken token)
        {
            await background.DOFade(1f,backgroundFadeInDuration).ToUniTask(cancellationToken: token);
            pages[_currentPage].gameObject.SetActive(true);

            var tasks = new List<UniTask>
            {
                pages[_currentPage].DOFade(1f, comicsFadeInDuration).ToUniTask(cancellationToken: token),
                _buttonImage.DOFade(1f, comicsFadeInDuration).ToUniTask(cancellationToken: token),
            };
            await UniTask.WhenAll(tasks);
            button.interactable = true;
        }
        private void GoToNextPage()
        {
           GoToNextPageAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask GoToNextPageAsync(CancellationToken token)
        {
            button.interactable = false;
            await pages[_currentPage].DOFade(0f, pageFadeDuration).ToUniTask(cancellationToken: token);
            pages[_currentPage].gameObject.SetActive(false);
            _currentPage++;
            if (_currentPage == pages.Length)
            {
                await EndComics(token);
                return;
            }
            
            await pages[_currentPage].DOFade(1f, pageFadeDuration).ToUniTask(cancellationToken: token);
            pages[_currentPage].gameObject.SetActive(true);
            button.interactable = true;
        }

        private async UniTask EndComics(CancellationToken token)
        {
            //todo ask gaydesigners what to do here
            await UniTask.Delay(1000, cancellationToken: token);
            Application.Quit();
            Debug.Log("END OF GAME");
        }
        private UniTask FadeOutAll(float duration, CancellationToken token)
        {
            var tasks = new List<UniTask>()
            {
                background.DOFade(0f, duration).ToUniTask(cancellationToken: token),
                pages[_currentPage].DOFade(0f, duration).ToUniTask(cancellationToken: token),
                _buttonImage.DOFade(0f, duration).ToUniTask(cancellationToken: token),
            };
            return UniTask.WhenAll(tasks);
        }
    }
}