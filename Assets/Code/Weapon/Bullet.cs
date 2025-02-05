using System;
using System.Collections;
using Code.Environment;
using UnityEngine;
using Zenject;

namespace Code.Weapon
{
    public class Bullet : MonoBehaviour, IDisposable
    {
        // Dependencies
        private IBulletManager _bulletManager;

        // Settings
        private const float BULLET_DAMAGE = 20f;
        private readonly LayerMask _wallLayer = 6;
        private readonly LayerMask _destroyableLayer = 7;
        private readonly LayerMask _markLayer = 8;
        
        private Coroutine _autoReturn;
        private const float BULLET_LIFETIME = 10;

        public class BulletFactory : PlaceholderFactory<Bullet> { }

        [Inject]
        public void Construct(IBulletManager bulletManager)
        {
            _bulletManager = bulletManager;
        }

        private void OnEnable()
        {
            _autoReturn = StartCoroutine(ReturnToPoolAfterDelay());
        }

        private void OnDisable()
        {
            StopCoroutine(_autoReturn);
        }

        private void OnCollisionEnter(Collision hittedObject)
        { 
            if (hittedObject.gameObject.TryGetComponent(out IDamageable damageable))
            {
                Debug.Log("Hit and damaged: " + damageable);
                StopCoroutine(_autoReturn);
                damageable.TakeDamage(BULLET_DAMAGE);
                _bulletManager.ReturnBulletToPool(this);
                return;
            }
            if (hittedObject.gameObject.layer == _wallLayer)
            {
                print("Hit " + hittedObject.gameObject.name);
                StopCoroutine(_autoReturn);
                CreateBulletImpactEffect(hittedObject);
                _bulletManager.ReturnBulletToPool(this);
                return;
            }
            if (hittedObject.gameObject.layer == _destroyableLayer)
            {
                print("Explode " + hittedObject.gameObject.name);
                StopCoroutine(_autoReturn);
                if (hittedObject.gameObject.TryGetComponent<Destroyable>(out var destroyable))
                {
                    destroyable.Explode();
                }
                _bulletManager.ReturnBulletToPool(this);
            }
            if (hittedObject.gameObject.layer == _markLayer)
            {
                print("Hit " + hittedObject.gameObject.name);
                StopCoroutine(_autoReturn);
                if (hittedObject.gameObject.TryGetComponent<Mark>(out var mark))
                {
                    mark.HitMark();
                }
                _bulletManager.ReturnBulletToPool(this);
            }
        }
        private IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(BULLET_LIFETIME);
            _bulletManager.ReturnBulletToPool(this);
        }
        private void CreateBulletImpactEffect(Collision hittedObject)
        {
            var contact = hittedObject.contacts[0];
            var hole = _bulletManager.GetBulletImpactEffect();

            hole.transform.SetPositionAndRotation(contact.point, Quaternion.LookRotation(contact.normal));
        }
        public void Dispose()
        {
            StopCoroutine(_autoReturn);
            Destroy(gameObject);
        }
    }
}