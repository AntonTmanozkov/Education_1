using UnityEngine;

namespace UnityExtentions
{
    public static class MathTools
    {
        /// <summary>
        /// Scales a game object around a point.
        /// </summary>
        /// <param name="target">the target game object.</param>
        /// <param name="pivot">the point the object would be scaled around.</param>
        /// <param name="newScale">the final scale.</param>
        public static void ScaleAround(this GameObject target, Vector3 pivot, Vector3 newScale)
        {
            Vector3 A = target.transform.localPosition;
            Vector3 B = pivot;

            Vector3 C = A - B; // diff from object pivot to desired pivot/origin

            float RS = newScale.x / target.transform.localScale.x; // relative scale factor

            // calc final position post-scale
            Vector3 FP = B + C * RS;

            // finally, actually perform the scale/translation
            target.transform.localScale = newScale;
            target.transform.localPosition = FP;
        }
    }
}