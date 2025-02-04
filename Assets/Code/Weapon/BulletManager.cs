using System;
using UnityEngine;
using Zenject;

namespace Code.Weapon
{
    public class BulletManager : MonoBehaviour, IBulletManager, IInitializable, IDisposable
    {
        [SerializeField] private GameObject bulletHolePrefab;

        // Dependencies
        private DiContainer _container;
        private Bullet.BulletFactory _bulletFactory;

        // Bullet pooling
        private Bullet[] _bulletPool;
        private const int BULLET_POOL_SIZE = 30;
        private int _currentBulletPoolIndex;

        // Bullet impact pooling
        private GameObject[] _bulletImpactPool;
        private const int BULLET_IMPACT_POOL_SIZE = 20;
        private int _currentBulletImpactPoolIndex;

        [Inject]
        public void Init(DiContainer container, Bullet.BulletFactory bulletFactory)
        {
            _container = container;
            _bulletFactory = bulletFactory;
        }
        public void Initialize()
        {
            _bulletPool = new Bullet[BULLET_POOL_SIZE];
            _bulletImpactPool = new GameObject[BULLET_IMPACT_POOL_SIZE];

            for (var i = 0; i < BULLET_POOL_SIZE; i++)
            {
                _bulletPool[i] = _bulletFactory.Create();
                _bulletPool[i].transform.SetParent(transform);
                _bulletPool[i].gameObject.SetActive(false);
            }
            for (var i = 0; i < BULLET_IMPACT_POOL_SIZE; i++)
            {
                _bulletImpactPool[i] = _container.InstantiatePrefab(bulletHolePrefab, transform);
                _bulletImpactPool[i].transform.SetParent(transform);
                _bulletImpactPool[i].SetActive(false);
            }
        }
        public Bullet GetBullet(Vector3 position)
        {
            var bullet = _bulletPool[_currentBulletPoolIndex];

            _currentBulletPoolIndex = (_currentBulletPoolIndex + 1) % BULLET_POOL_SIZE;

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

            _currentBulletImpactPoolIndex = (_currentBulletImpactPoolIndex + 1) % BULLET_IMPACT_POOL_SIZE;

            impactEffect.SetActive(true);

            return impactEffect;
        }
        public void Dispose()
        {
            if (_bulletPool != null)
            {
                for (var i = 0; i < BULLET_POOL_SIZE; i++)
                {
                    if (_bulletPool[i] != null)
                    {
                        Destroy(_bulletPool[i].gameObject);
                    }
                }
            }

            if (_bulletImpactPool == null) return;
        
            for (var i = 0; i < BULLET_IMPACT_POOL_SIZE; i++)
            {
                if (_bulletImpactPool[i] != null)
                {
                    Destroy(_bulletImpactPool[i]);
                }
            }
        
        }
    }
}