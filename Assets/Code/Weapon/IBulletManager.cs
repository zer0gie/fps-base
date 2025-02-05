using UnityEngine;

namespace Code.Weapon
{
    public interface IBulletManager
    {
        Bullet GetBullet(Vector3 position);
        GameObject GetBulletImpactEffect();
        void ReturnBulletToPool(Bullet bullet);
    }
}