using UnityEngine;

namespace UnityExtentions
{
    public class DestroyObserver : MonoBehaviour
    {
        [SerializeField] DestroyEvent _target;

        private void OnEnable()
        {
            _target.Destroyed.AddListener(OnTargetDestroy);
        }

        private void OnDisable()
        {
            _target.Destroyed.RemoveListener(OnTargetDestroy);
        }

        private void OnTargetDestroy()
        {
            Destroy(gameObject);
        }
    }
}
