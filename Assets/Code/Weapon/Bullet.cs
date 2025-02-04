using System;
using UnityEngine;
using Zenject;

namespace Code.Weapon
{
    public class Bullet : MonoBehaviour, IDisposable
    {
        // Dependencies
        private IBulletManager _bulletManager;

        // Settings
        private readonly float _damage9M = 20f;
        private readonly LayerMask _wallLayer = 6;
        private readonly LayerMask _destroyableLayer = 7;

        public class BulletFactory : PlaceholderFactory<Bullet> { }

        [Inject]
        public void Construct(IBulletManager bulletManager)
        {
            _bulletManager = bulletManager;
        }
        private void OnCollisionEnter(Collision hittedObject)
        { 
            if (hittedObject.gameObject.TryGetComponent(out IDamageable damageable))
            {
                Debug.Log("Hit and damaged: " + damageable);
                damageable.TakeDamage(_damage9M);
                return;
            }
            if (hittedObject.gameObject.layer == _wallLayer)
            {
                print("Hit " + hittedObject.gameObject.name);
                CreateBulletImpactEffect(hittedObject);
                return;
            }
            if (hittedObject.gameObject.layer == _destroyableLayer)
            {
                print("Explode " + hittedObject.gameObject.name);
                if (hittedObject.gameObject.TryGetComponent<Destroyable>(out var destroyable))
                {
                    destroyable.Explode();
                }
            }
        }
        private void CreateBulletImpactEffect(Collision hittedObject)
        {
            var contact = hittedObject.contacts[0];

            var hole = _bulletManager.GetBulletImpactEffect();

            hole.transform.SetPositionAndRotation(contact.point, Quaternion.LookRotation(contact.normal));
        }
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}