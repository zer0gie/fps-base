using Animancer;
using UnityEngine;

namespace Resources.Code.ScriptableObjects;

[CreateAssetMenu()]
public class WeaponSettingsSO : ScriptableObject
{
    public string weaponName;
    public int clipSize;
    public int stackSize;
    public float spreadIntensity;
    public float maxShootDistance;
    public float bulletVelocity;
    public AudioClip shootingSound;
    public AudioClip reloadSound;
    public AudioClip emptyMagazineSound;
    public ClipTransition idleAnimation;
    public ClipTransition shootAnimation;
    public ClipTransition reloadAnimation;
    public ClipTransition takeAnimation;
}