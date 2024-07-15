using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityExtentions.Random
{
    public static class RandomTools
    {
        public static int RandomIndex(this Array array)
        {
            return UnityEngine.Random.Range(0, array.Length);
        }

        public static T RandomElement<T>(this T[] array)
        {
            if (array == null) 
            {
                throw new ArgumentException("Array is null!");
            }
            if (array.Length == 0) 
            {
                throw new ArgumentException("Array is empty!");
            }

            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static Vector3 RandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
        }

        public static T RandomElement<T>(this IEnumerable<T> @this)
        {
            if (@this.Count() == 0)
            {
                throw new ArgumentException("IEnumerable is empty!");
            }

            return @this.ElementAt(UnityEngine.Random.Range(0, @this.Count()));
        }
    }
}