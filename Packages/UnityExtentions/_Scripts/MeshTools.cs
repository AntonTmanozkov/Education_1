using UnityEngine;

namespace UnityExtentions
{

    public static class MeshTools
    {
        public static class Volume
        {
            /// <summary>
            /// Code source https://stackoverflow.com/questions/57236085/get-volume-of-an-object-in-unity3d
            /// </summary>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <param name="p3"></param>
            /// <returns></returns>
            private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                float v321 = p3.x * p2.y * p1.z;
                float v231 = p2.x * p3.y * p1.z;
                float v312 = p3.x * p1.y * p2.z;
                float v132 = p1.x * p3.y * p2.z;
                float v213 = p2.x * p1.y * p3.z;
                float v123 = p1.x * p2.y * p3.z;
                return 1.0f / 6.0f * (-v321 + v231 + v312 - v132 - v213 + v123);
            }

            /// <summary>
            /// Calculates the volume using signed volume of the triangles. Not affected by scale.
            /// </summary>
            /// <param name="mesh">Target mesh</param>
            /// <returns></returns>
            public static float Get(Mesh mesh)
            {
                float volume = 0;
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                for (int i = 0; i < mesh.triangles.Length; i += 3)
                {
                    Vector3 p1 = vertices[triangles[i + 0]];
                    Vector3 p2 = vertices[triangles[i + 1]];
                    Vector3 p3 = vertices[triangles[i + 2]];
                    volume += SignedVolumeOfTriangle(p1, p2, p3);
                }
                return Mathf.Abs(volume);
            }

            /// <summary>
            /// Calculates the volume using signed volume of the triangles.
            /// </summary>
            /// <param name="mesh">Target mesh.</param>
            /// <returns></returns>
            public static float Get(Mesh mesh, float scale)
            {
                return Get(mesh) * scale;
            }
        }

        public static class Center
        {
            /// <summary>
            /// Local center of the mesh
            /// </summary>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public static Vector3 Get(Mesh mesh)
            {
                return mesh.bounds.center;
            }

            /// <summary>
            /// Global center of the mesh
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="transform"></param>
            /// <returns></returns>
            public static Vector3 Get(Mesh mesh, Transform transform)
            {
                return transform.TransformPoint(mesh.bounds.center);
            }

            /// <summary>
            /// The center of the mesh in local transform.
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="transform"></param>
            /// <returns></returns>
            public static Vector3 GetDifferenceBetweenMeshCenterAndPosition(Mesh mesh, Transform transform)
            {
                return transform.position - Get(mesh, transform);
            }
        }
    }
}