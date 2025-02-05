using System.Collections.Generic;
using UnityEngine;

namespace Code.Environment
{
    public class Destroyable : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> partsList = new();

        public void Explode()
        {
            if (TryGetComponent(out Collider colliders))
            {
                colliders.enabled = false;
            }
            foreach (var part in partsList)
            {
                part.isKinematic = false;
            }
        }
    }
}