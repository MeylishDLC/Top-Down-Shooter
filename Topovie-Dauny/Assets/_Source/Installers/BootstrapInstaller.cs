using Analytics;
using Core.LevelSettings;
using SoundSystem;
using TMPro;
using UI.Core.Loading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SceneLoader = Core.SceneManagement.SceneLoader;

namespace Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoader sceneLoaderPrefab;
        [SerializeField] private AudioManager audioManagerPrefab;
        [SerializeField] private AnalyticsManager analyticsManagerPrefab;
        [SerializeField] private LoadingTipsConfig loadingTipsConfig;
        
        [Header("Loading Elements")]
        [SerializeField] private Canvas canvasPrefab;
        [SerializeField] private RectTransform loadingScreenPrefab;
        
        private AudioManager _audioManager;
        private AnalyticsManager _analyticsManager;
        public override void InstallBindings()
        {
            BindAnalyticsManager();
            BindAudioManager();
            BindLevelSave();
            
            BindLoadingTips();
            BindSceneLoader();
        }
        private void BindAudioManager()
        {
            _audioManager = Container.InstantiatePrefabForComponent<AudioManager>(audioManagerPrefab);
            Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
        }
        private void BindLevelSave()
        {
            Container.Bind<LevelSave>().AsSingle().WithArguments(_analyticsManager);
        }
        private void BindSceneLoader()
        {
            var canvas = Container.InstantiatePrefabForComponent<Canvas>(canvasPrefab);
            var screen = Container.InstantiatePrefab(loadingScreenPrefab, canvas.transform);
            var components = new LoadingScreenComponents(screen.GetComponent<RectTransform>(),
                screen.GetComponentInChildren<Slider>(), screen.GetComponentInChildren<TMP_Text>());
            
            Container.Bind<LoadingScreenComponents>().FromInstance(components).AsSingle();
            
            var loader = Container.InstantiatePrefabForComponent<SceneLoader>(sceneLoaderPrefab);
            Container.Bind<SceneLoader>().FromInstance(loader).AsSingle();
        }
        private void BindAnalyticsManager()
        {
            _analyticsManager = Container.InstantiatePrefabForComponent<AnalyticsManager>(analyticsManagerPrefab);
            Container.Bind<AnalyticsManager>().FromInstance(_analyticsManager).AsSingle();
        }
        private void BindLoadingTips()
        {
            Container.BindInstance(loadingTipsConfig).AsSingle();
            Container.Bind<LoadingTips>().AsSingle();
        }
    }
}