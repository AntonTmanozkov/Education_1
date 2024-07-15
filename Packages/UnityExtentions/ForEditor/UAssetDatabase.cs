using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityExtentions.ForEditor
{
    public static class UAssetDatabase
    {
        public static T[] LoadAllAssetsWithType<T>() where T : Object
        {
            string[] objects = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> result = new();
            foreach (string obj in objects)
            {
                string path = AssetDatabase.GUIDToAssetPath(obj);
                result.Add(AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T);
            }

            return result.ToArray();
        }
    }
}
