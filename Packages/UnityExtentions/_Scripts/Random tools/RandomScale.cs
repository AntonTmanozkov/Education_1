using UnityEngine;

namespace UnityExtentions.Random
{
    public class RandomScale : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool RandomX;
        [SerializeField] private bool RandomY;
        [SerializeField] private bool RandomZ;

        [Header("Range")]
        [SerializeField] private Range X = new(0.5f, 2);
        [SerializeField] private Range Y = new(0.5f, 2);
        [SerializeField] private Range Z = new(0.5f, 2);

        private void Start()
        {
            Scale();
        }

        public void Scale() 
        {
            if (RandomX) transform.localScale = new(X, transform.localScale.y, transform.localScale.z);
            if (RandomY) transform.localScale = new(transform.localScale.x, Y, transform.localScale.z);
            if (RandomZ) transform.localScale = new(transform.localScale.x, transform.localScale.y, Z);
        }
    }
}
