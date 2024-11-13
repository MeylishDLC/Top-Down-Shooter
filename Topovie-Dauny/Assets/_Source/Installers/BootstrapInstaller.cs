using Core.LevelSettings;
using Core.SceneManagement;
using UnityEngine;
using Zenject;
using SceneLoader = Core.SceneManagement.SceneLoader;

namespace Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindLevelSave();
        }
        private void BindLevelSave()
        {
            Container.Bind<LevelSave>().AsSingle();
        }
    }
}