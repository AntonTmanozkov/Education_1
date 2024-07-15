using UnityEngine;

namespace UnityExtentions
{
    [ExecuteAlways]
    public class SpriteCopy : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _source;
        [SerializeField] private SpriteRenderer _target;

        private void LateUpdate()
        {
            if (_source.sprite != _target.sprite) 
            {
                _target.sprite = _source.sprite;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _target ??= GetComponent<SpriteRenderer>();
        }
#endif
    }
}
