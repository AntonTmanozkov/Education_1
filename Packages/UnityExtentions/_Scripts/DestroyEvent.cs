using UnityEngine;
using UnityEngine.Events;

namespace UnityExtentions
{
    public class DestroyEvent : MonoBehaviour 
    {
        [SerializeField] UnityEvent _destroyed;

        public UnityEvent Destroyed => _destroyed;

        private void OnDestroy()
        {
            _destroyed.Invoke();
        }
    }
}
