using System;
using UnityEngine;
using Zenject;
using static Resources.Code.Weapon.Bullet;

namespace Resources.Code.Weapon;

public class ProjectileManager : MonoBehaviour, IBulletManager, IInitializable, IDisposable
{
    [SerializeField] private GameObject bulletHolePrefab;

    // Dependencies
    private DiContainer _container;
    private BulletFactory _bulletFactory;

    // Bullet pooling
    private Bullet[] _bulletPool;
    private const int BulletPoolSize = 30;
    private int _currentBulletPoolIndex;

    // Bullet impact pooling
    private GameObject[] _bulletImpactPool;
    private const int BulletImpactPoolSize = 20;
    private int _currentBulletImpactPoolIndex;

    [Inject]
    public void Init(DiContainer container, BulletFactory bulletFactory)
    {
        _container = container;
        _bulletFactory = bulletFactory;
    }
    public void Initialize()
    {
        _bulletPool = new Bullet[BulletPoolSize];
        _bulletImpactPool = new GameObject[BulletImpactPoolSize];

        for (var i = 0; i < BulletPoolSize; i++)
        {
            _bulletPool[i] = _bulletFactory.Create();
            _bulletPool[i].transform.SetParent(transform);
            _bulletPool[i].gameObject.SetActive(false);
        }
        for (var i = 0; i < BulletImpactPoolSize; i++)
        {
            _bulletImpactPool[i] = _container.InstantiatePrefab(bulletHolePrefab, transform);
            _bulletImpactPool[i].transform.SetParent(transform);
            _bulletImpactPool[i].SetActive(false);
        }
    }
    public Bullet GetBullet(Vector3 position)
    {
        var bullet = _bulletPool[_currentBulletPoolIndex];

        _currentBulletPoolIndex = (_currentBulletPoolIndex + 1) % BulletPoolSize;

        bullet.transform.SetPositionAndRotation(position, Quaternion.identity);
        
        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        bullet.gameObject.SetActive(true);

        return bullet;
    }
    public GameObject GetBulletImpactEffect()
    {
        var impactEffect = _bulletImpactPool[_currentBulletImpactPoolIndex];

        _currentBulletImpactPoolIndex = (_currentBulletImpactPoolIndex + 1) % BulletImpactPoolSize;

        impactEffect.SetActive(true);

        return impactEffect;
    }
    public void Dispose()
    {
        if (_bulletPool != null)
        {
            for (var i = 0; i < BulletPoolSize; i++)
            {
                if (_bulletPool[i] != null)
                {
                    Destroy(_bulletPool[i].gameObject);
                }
            }
        }

        if (_bulletImpactPool == null) return;
        
        for (var i = 0; i < BulletImpactPoolSize; i++)
        {
            if (_bulletImpactPool[i] != null)
            {
                Destroy(_bulletImpactPool[i]);
            }
        }
        
    }
}