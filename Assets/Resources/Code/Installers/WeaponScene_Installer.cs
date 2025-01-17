using Resources.Code.UI;
using Resources.Code.Weapon;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
#pragma warning disable CS8604 // Possible null reference argument.

public class WeaponSceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject m1911;
    [SerializeField] private GameObject ak74;
    [SerializeField] private GameObject bennelliM4;

    [SerializeField] private GameObject bulletManagerPrefab;
    [SerializeField] private GameObject uiManagerPrefab;

    [SerializeField] private GameObject bulletPrefab;

    public override void InstallBindings()
    {
        Container.Bind<Weapon>().WithId("M1911").FromComponentInNewPrefab(m1911).AsCached();
        Container.Bind<Weapon>().WithId("AK74").FromComponentInNewPrefab(ak74).AsCached();
        Container.Bind<Weapon>().WithId("BENNELLIM4").FromComponentInNewPrefab(bennelliM4).AsCached();

        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<WeaponManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<UIManager>().FromComponentInNewPrefab(uiManagerPrefab).AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<ProjectileManager>().FromComponentInNewPrefab(bulletManagerPrefab).AsSingle().NonLazy();

        Container.BindFactory<Bullet, Bullet.BulletFactory>()
            .FromComponentInNewPrefab(bulletPrefab)
            .AsCached();
    }
}