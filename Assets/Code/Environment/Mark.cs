using DG.Tweening;
using UnityEngine;

namespace Code.Environment
{
    public class Mark : MonoBehaviour
    {
        private const float DURATION = 1f;
        private const float MARK_HIT_DELAY = 3f;
        [SerializeField] private Ease easeType = Ease.OutElastic;

        private bool _hasFallen;
    
        public void HitMark()
        {
            if (_hasFallen) return;
            _hasFallen = true;

            transform.DOLocalRotate(new Vector3(90f, 310f, 0f), DURATION)
                .SetEase(easeType)
                .OnComplete(() => {
                    transform.DOLocalRotate(new Vector3(2f, 310f, 0f), DURATION)
                        .SetDelay(MARK_HIT_DELAY)
                        .SetEase(easeType)
                        .OnComplete(() => {
                            _hasFallen = false;
                        });
                });
        }
    }
}
