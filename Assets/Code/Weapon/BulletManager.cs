using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Weapon
{
    public class BulletManager : MonoBehaviour, IBulletManager, IInitializable, IDisposable
    {
        [SerializeField] private GameObject bulletHolePrefab;

        // Dependencies
        private DiContainer _container;
        private Bullet.BulletFactory _bulletFactory;

        // Bullet pooling
        private Queue<Bullet> _availableBullets;
        private const int BULLET_POOL_SIZE = 30;

        // Bullet impact pooling
        private Queue<GameObject> _availableImpacts;
        private const int BULLET_IMPACT_POOL_SIZE = 20;

        [Inject]
        public void Init(DiContainer container, Bullet.BulletFactory bulletFactory)
        {
            _container = container;
            _bulletFactory = bulletFactory;
        }
        public void Initialize()
        {
            _availableBullets = new Queue<Bullet>(BULLET_POOL_SIZE);
            _availableImpacts = new Queue<GameObject>(BULLET_IMPACT_POOL_SIZE);

            for (var i = 0; i < BULLET_POOL_SIZE; i++)
            {
                var bullet = _bulletFactory.Create();
                bullet.transform.SetParent(transform);
                bullet.gameObject.SetActive(false);
                _availableBullets.Enqueue(bullet);
            }

            for (var i = 0; i < BULLET_IMPACT_POOL_SIZE; i++)
            {
                var impact = _container.InstantiatePrefab(bulletHolePrefab, transform);
                impact.transform.SetParent(transform);
                impact.SetActive(false);
                _availableImpacts.Enqueue(impact);
            }
        }
        public Bullet GetBullet(Vector3 position)
        {
            Bullet bullet;
            if (_availableBullets.Count == 0)
            {
                bullet = _bulletFactory.Create();
                bullet.transform.SetParent(transform);
            }
            else
            {
                bullet = _availableBullets.Dequeue();
            }

            bullet.transform.SetPositionAndRotation(position, Quaternion.identity);
        
            if (bullet.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            bullet.gameObject.SetActive(true);

            return bullet;
        }
        public void ReturnBulletToPool(Bullet bullet)
        {
            if (!bullet) return;
            
            bullet.gameObject.SetActive(false);
            _availableBullets.Enqueue(bullet);
        }
        public GameObject GetBulletImpactEffect()
        {
            GameObject impact;
            if (_availableImpacts.Count == 0)
            {
                impact = _container.InstantiatePrefab(bulletHolePrefab, transform);
                impact.transform.SetParent(transform);
            }
            else
            {
                impact = _availableImpacts.Dequeue();
            }

            impact.SetActive(true);
            ReturnImpactToPoolAfterDelay(impact, this.GetCancellationTokenOnDestroy()).Forget();
            return impact;
        }

        private async UniTask ReturnImpactToPoolAfterDelay(GameObject impact, CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(10, cancellationToken: cancellationToken);
            if (!impact) return;
            
            impact.SetActive(false);
            _availableImpacts.Enqueue(impact);   
        }
        public void Dispose()
        {
            while (_availableBullets?.Count > 0)
            {
                var bullet = _availableBullets.Dequeue();
                if (bullet != null)
                {
                    Destroy(bullet.gameObject);
                }
            }

            while (_availableImpacts?.Count > 0)
            {
                var impact = _availableImpacts.Dequeue();
                if (impact != null)
                {
                    Destroy(impact);
                }
            }
        }
    }
}