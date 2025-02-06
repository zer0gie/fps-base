using Code.Audio;
using Code.PlayerInput;
using Zenject;

namespace Code.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AudioManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle().NonLazy();
        }
    }
}