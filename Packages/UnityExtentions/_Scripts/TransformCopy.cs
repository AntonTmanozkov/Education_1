using UnityEngine;

namespace UnityExtentions
{
    [ExecuteAlways]
    public class TransformCopy : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [Header("Setting")]
        [SerializeField] private bool CopyPosition;
        [SerializeField] private bool CopyRotate;
        [SerializeField] private bool CopyScale;
        [SerializeField] private bool DetachFromParent;

        private void Awake()
        {
            if (DetachFromParent && Application.IsPlaying(this)) 
            {
                transform.SetParent(null);
            }
        }

        private void LateUpdate()
        {
            if (_target.position != transform.position && CopyPosition)
                transform.position = _target.position;

            if (_target.rotation != transform.rotation && CopyRotate)
                transform.rotation = _target.rotation;

            if (_target.localScale != transform.localScale && CopyScale)
                transform.localScale = _target.localScale;
        }
    }
}
