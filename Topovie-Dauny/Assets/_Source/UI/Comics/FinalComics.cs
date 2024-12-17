using System;
using System.Collections.Generic;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Core.InputSystem;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using Enemies.Boss;
using FMOD.Studio;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Comics
{
    public class FinalComics: MonoBehaviour
    {
        [SerializeField] private BossLeo bossLeo;
        [SerializeField] private Image[] pages;
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private float delayBeforeComicsStart = 1f;
        [SerializeField] private float backgroundFadeInDuration = 0.25f;
        [SerializeField] private float comicsFadeInDuration = 0.25f;
        [SerializeField] private float pageFadeDuration = 0.25f;

        private Image _buttonImage;
        private int _currentPage;
        private CancellationToken _destroyCancellationToken;
        private InputListener _inputListener;
        private AudioManager _audioManager;
        private SceneLoader _sceneLoader;
        
        [Inject]
        public void Construct(InputListener inputListener, AudioManager audioManager, SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _audioManager = audioManager;
            _inputListener = inputListener;
        }
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
        private void ShowComics()
        {
            _inputListener.SetInput(false);
            _audioManager.ChangeMusic(_audioManager.FMODEvents.EndingMusic, STOP_MODE.ALLOWFADEOUT);
            FadeInAsync(_destroyCancellationToken).Forget();
        }
        private async UniTask FadeInAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeComicsStart), cancellationToken: token);
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
            if (_currentPage >= pages.Length)
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
            await UniTask.Delay(1000, cancellationToken: token);
            //todo fadeout
            _sceneLoader.LoadSceneAsync(0).Forget();
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