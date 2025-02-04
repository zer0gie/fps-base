using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Helpers
{
    public class FPSLimiter : MonoBehaviour
    {
        [SerializeField]
        private int targetFrameRate = 60; 

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }

        private void OnValidate()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}