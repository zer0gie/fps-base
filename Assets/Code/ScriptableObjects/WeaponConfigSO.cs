using Animancer;
using UnityEngine;

namespace Code.ScriptableObjects
{
    [CreateAssetMenu()]
    public class WeaponConfigSO : ScriptableObject
    {
        public string weaponName;
        public int clipSize;
        public int stackSize;
        public float spreadIntensity;
        public float maxShootDistance;
        public float bulletVelocity;
        public float recoilAmount;
        public AudioClip shootingSound;
        public AudioClip reloadSound;
        public AudioClip emptyMagazineSound;
        public AudioClip equipSound;
        public ClipTransition idleAnimation;
        public ClipTransition shootAnimation;
        public ClipTransition reloadAnimation;
        public ClipTransition takeAnimation;
    }
}