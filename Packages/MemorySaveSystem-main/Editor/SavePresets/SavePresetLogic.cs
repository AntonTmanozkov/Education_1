#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace MemorySaveSystem.Editor.SavePresets
{
    internal class SavePresetLogic
    {
        public string CreatePreset(string name = "new preset.json") 
        {
            string path = AssetDatabase.GenerateUniqueAssetPath(MemorySettings.PRESETS_RESOURSES_PATH + name);
            FileStream stream = File.Create(Application.dataPath + path.Split("Assets")[1]);
            stream.Close();
            AssetDatabase.ImportAsset(path);
            return path;
        }

        public void RemovePreset(TextAsset preset) 
        {
            string assetPath = AssetDatabase.GetAssetPath(preset);
            AssetDatabase.DeleteAsset(assetPath);
        }

        public void RenamePreset(TextAsset preset, string newName) 
        {
            if (string.IsNullOrEmpty(newName)) 
            {
                Debug.LogError($"Name \"{newName}\" is not valid");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(preset);
            AssetDatabase.RenameAsset(assetPath, newName);
        }

        public void CreateCopy(TextAsset preset) 
        {
            string path = CreatePreset(preset.name + " copy.json");
            AssetDatabase.ImportAsset(path);
        }

        public void LoadFromCurrentSaveIn(TextAsset preset) 
        {
            using (StreamReader stream = new(Memory.Path))
            {
                string save = stream.ReadToEnd();
                File.WriteAllText(Application.dataPath + AssetDatabase.GetAssetPath(preset).Split("Assets")[1], save);
            }
            EditorUtility.SetDirty(preset);
            AssetDatabase.SaveAssetIfDirty(preset);
            AssetDatabase.Refresh();
        }

        public void ApplyPreset(TextAsset preset) 
        {
            using (StreamWriter writer = new(Memory.Path))
            {
                writer.Write(preset.text);
                writer.Close();
            }
        }
    }
}
#endif