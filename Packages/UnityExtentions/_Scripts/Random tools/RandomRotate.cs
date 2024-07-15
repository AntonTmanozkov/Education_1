using UnityEngine;

namespace UnityExtentions.Random
{
    public class RandomRotate : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool RandomX;
        [SerializeField] private bool RandomY;
        [SerializeField] private bool RandomZ;

        [Header("Range")]
        [SerializeField] private Range X = new(0, 360);
        [SerializeField] private Range Y = new(0, 360);
        [SerializeField] private Range Z = new(0, 360);

        private void Start()
        {
            Rotate();
        }

        public void Rotate() 
        {
            if (RandomX) transform.eulerAngles = new(X, transform.eulerAngles.y, transform.eulerAngles.z);
            if (RandomY) transform.eulerAngles = new(transform.eulerAngles.x, Y, transform.eulerAngles.z);
            if (RandomZ) transform.eulerAngles = new(transform.eulerAngles.x, transform.eulerAngles.y, Z);
        }
    }
}
