using UnityEngine;

namespace Resources.Code.Weapon;

public interface IBulletManager
{
    Bullet GetBullet(Vector3 position);
    GameObject GetBulletImpactEffect();
}