using Core.InputSystem;
using Core.SceneManagement;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MenusInstaller : MonoInstaller
    {
        [SerializeField] private InputListener inputListener;

        public override void InstallBindings()
        {
            BindInputListener();
        }
        private void BindInputListener()
        {
            Container.Bind<InputListener>().FromInstance(inputListener).AsSingle();
        }
    }
}
