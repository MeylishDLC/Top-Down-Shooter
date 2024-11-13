using Core.InputSystem;
using Core.SceneManagement;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MenusInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private InputListener inputListener;

        public override void InstallBindings()
        {
            BindSceneLoader();
            BindInputListener();
        }
        private void BindSceneLoader()
        {
            Container.Bind<SceneLoader>().FromInstance(sceneLoader).AsSingle();
        }
        private void BindInputListener()
        {
            Container.Bind<InputListener>().FromInstance(inputListener).AsSingle();
        }
    }
}
