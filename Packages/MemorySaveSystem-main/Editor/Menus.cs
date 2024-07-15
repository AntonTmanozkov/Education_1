#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MemorySaveSystem 
{
    public static class Menus
    {
        [MenuItem("Memory/Clear saves")]
        private static void ClearSaves()
        {
            Directory.Delete(Application.persistentDataPath, true);
            UnityEngine.Debug.Log("Saves was deleted");
        }

        [MenuItem("Memory/Open save file")]
        private static void OpenSave()
        {
            Process.Start(Memory.Path);
        }
    }
}
#endif