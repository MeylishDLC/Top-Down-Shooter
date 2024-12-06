using Core.LevelSettings;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SceneLoader = Core.SceneManagement.SceneLoader;

namespace Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform loadingScreenPrefab;
        [SerializeField] private SceneLoader sceneLoaderPrefab;
        [SerializeField] private Canvas canvasPrefab;
        [SerializeField] private AudioManager audioManagerPrefab;
        public override void InstallBindings()
        {
            BindAudioManager();
            BindLevelSave();
            BindSceneLoader();
        }
        private void BindAudioManager()
        {
            var audioManager = Container.InstantiatePrefabForComponent<AudioManager>(audioManagerPrefab);
            Container.Bind<AudioManager>().FromInstance(audioManager).AsSingle();
        }
        private void BindLevelSave()
        {
            Container.Bind<LevelSave>().AsSingle();
        }
        private void BindSceneLoader()
        {
            var canvas = Container.InstantiatePrefabForComponent<Canvas>(canvasPrefab);
            var screen = Container.InstantiatePrefab(loadingScreenPrefab, canvas.transform);
            var loader = Container.InstantiatePrefabForComponent<SceneLoader>(sceneLoaderPrefab);
            
            loader.Construct(screen.GetComponent<RectTransform>(), screen.GetComponentInChildren<Slider>());
            Container.Bind<SceneLoader>().FromInstance(loader).AsSingle();
        }
    }
}