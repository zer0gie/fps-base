using Code.Audio;
using Code.PlayerInput;
using Code.UI;
using UnityEngine;
using Zenject;

namespace Code.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private GameObject uiManagerPrefab;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AudioManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<UIManager>().FromComponentInNewPrefab(uiManagerPrefab).AsSingle().NonLazy();
        }
    }
}