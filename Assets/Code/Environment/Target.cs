using UnityEngine;

namespace Code.Environment
{
    public class Target : MonoBehaviour, IDamageable
    {
        public void TakeDamage(float f)
        {
            Debug.Log("Give a hit: " + f + " HP");
        }
    }
}