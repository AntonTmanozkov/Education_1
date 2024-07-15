using System.Linq;
using UnityEngine;

namespace UnityExtentions
{
    public static class ColliderTools
    {
        public static class PointsInCollider
        {
            public static (bool, int) SomeInside(Collider collider, Vector3[] points)
            {
                int pointsOutside = 0;
                bool areInside = false;
                foreach (var vertex in points)
                {
                    var isInside = PointInCollider.IsInside(collider, vertex);
                    if (isInside == true)
                    {
                        areInside = true;
                    }
                    else
                    {
                        pointsOutside++;
                    }
                }

                return (areInside, pointsOutside);
            }

            /// <summary>
            /// Checks if the points are inside the collider using ray casting.
            /// </summary>
            /// <param name="collider">Collider that is checked against.</param>
            /// <param name="points">Points in world space.</param>
            /// <returns>True if all the points are inside the collider, if even one point not inside return false.</returns>
            public static bool AllInside(Collider collider, Vector3[] points)
            {
                foreach (var vertex in points)
                {
                    var isInside = PointInCollider.IsInside(collider, vertex);
                    if (isInside == false)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static class PointInCollider
        {
            public static bool IsInside(Collider other, Vector3 point)
            {
                Vector3 from = (Vector3.up * 5000f);
                Vector3 dir = (point - from).normalized;
                float dist = Vector3.Distance(from, point);
                //fwd
                int hit_count = Cast_Till(from, point, other);
                //back
                dir = (from - point).normalized;
                hit_count += Cast_Till(point, point + (dir * dist), other);

                if (hit_count % 2 == 1)
                {
                    return (true);
                }
                return (false);
            }

            private static int Cast_Till(Vector3 from, Vector3 to, Collider other)
            {
                int counter = 0;
                Vector3 dir = (to - from).normalized;
                float dist = Vector3.Distance(from, to);
                bool Break = false;
                while (!Break)
                {
                    Break = true;
                    RaycastHit[] hit = Physics.RaycastAll(from, dir, dist);
                    for (int tt = 0; tt < hit.Length; tt++)
                    {
                        if (hit[tt].collider == other)
                        {
                            counter++;
                            from = hit[tt].point + dir.normalized * .001f;
                            dist = Vector3.Distance(from, to);
                            Break = false;
                            break;
                        }
                    }
                }
                return (counter);
            }
        }

        public static class MeshInCollider
        {
            /// <summary>
            /// Checks if the target object is inside the collider using ray casting.
            /// </summary>
            /// <param name="collider">Collider that is checked against.</param>
            /// <param name="targetMesh">The object that has mesh filter.</param>
            /// <returns>True if is the target inside the collider completely, otherwise false.</returns>
            public static bool Internal(Collider collider, Mesh targetMesh, Transform targetTransform)
            {
                if (collider == null)
                {
                    Debug.Log("Collider is null!");
                    return false;
                }
                if (collider.enabled == false)
                {
                    Debug.LogWarning("Collider disabled!");
                    return false;
                }
                var verticies = targetMesh.vertices;

                return PointsInCollider.AllInside(collider, verticies.Select(point => targetTransform.TransformPoint(point)).ToArray());
            }

            /// <summary>
            /// Checks if the collider has points inside of collider
            /// </summary>
            /// <param name="mainCollider">The main collider</param>
            /// <param name="targetMesh">The mesh you check against</param>
            /// <param name="targetTransform">the transform that is attached to the mesh</param>
            /// <returns>true and amount of points outside, otherwise false and 0</returns>
            public static (bool, int) PartlyInternal(Collider mainCollider, Mesh targetMesh, Transform targetTransform)
            {
                if (mainCollider == null)
                {
                    Debug.LogWarning("Collider is null!");
                    return (false, 0);
                }
                if (mainCollider.enabled == false)
                {
                    Debug.LogWarning("Collider disabled!");
                    return (false, 0);
                }

                var verticies = targetMesh.vertices;

                return PointsInCollider.SomeInside(mainCollider, verticies.Select(point => targetTransform.TransformPoint(point)).ToArray());
            }
        }
    }
}