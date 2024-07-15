using UnityEditor;
using UnityEngine;

namespace MemorySaveSystem
{
    public class MemorySettings : ScriptableObject
    {
        public const string RESOURCES_PATH = "Assets/Resources/Memory/";
        public const string PRESETS_RESOURSES_PATH = "Assets/Resources/Memory/Save presets/";
        private const string FILE_NAME = "Memory settings.asset";
        private const string RESOURCES_NAME = "Memory settings";
#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        public static void Init()
        {
            if (AssetDatabase.IsValidFolder("Assets/Resources") == false)
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            if (AssetDatabase.IsValidFolder("Assets/Resources/Memory") == false)
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Memory");
            }

            if (AssetDatabase.IsValidFolder(PRESETS_RESOURSES_PATH) == false) 
            {
                AssetDatabase.CreateFolder("Assets/Resources/Memory", "Save presets");
            }

            if (Instance != null)
            {
                return;
            }

            Debug.Log($"{Memory.MESSAGE_PREFFIX} Memory settings file was created at \"{RESOURCES_PATH}\"");
            MemorySettings memorySettings = CreateInstance<MemorySettings>();
            AssetDatabase.CreateAsset(memorySettings, RESOURCES_PATH + FILE_NAME);
        }

        public void ShowSave() 
        {
            foreach (string key in Memory.saves.Keys) 
            {
                Debug.Log($"Key: \'{key}\'\nValue: \'{Memory.saves[key]}\'");
            }
        }
#endif

        public static MemorySettings Instance => Resources.Load<MemorySettings>("Memory/"+RESOURCES_NAME);

        [SerializeField] private DebugMode _debugMode;

        [SerializeField] private CloudMode _cloudSaveMode;
        public CloudMode CloudSaveMode => _cloudSaveMode;

        public DebugMode DebugModeValue => _debugMode;


        public enum DebugMode 
        {
            None = 0, 
            Defualt = 1,
            Deep = 2
        }

        public enum CloudMode 
        {
            None,
#if UNITY_CLOUD_SAVE
            Unity
#endif
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(MemorySettings))]
        private class CEditor : Editor 
        {
            public MemorySettings settings => (MemorySettings)target;
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (GUILayout.Button("Show saves")) 
                {
                    settings.ShowSave();
                }
            }
        }
#endif
    }
}