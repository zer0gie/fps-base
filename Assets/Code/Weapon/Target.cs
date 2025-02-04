using UnityEngine;

namespace Code.Weapon
{
    public class Target : MonoBehaviour, IDamageable
    {
        public void TakeDamage(float f)
        {
            Debug.Log("Give a hit: " + f + " HP");
        }
    }
}