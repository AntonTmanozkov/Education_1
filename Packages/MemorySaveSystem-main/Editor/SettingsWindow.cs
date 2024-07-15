#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MemorySaveSystem
{
    public class SettingsWindow : EditorWindow
    {
        [MenuItem("Memory/Settings")]
        private static void ClearSaves()
        {
            SettingsWindow window = GetWindow<SettingsWindow>();
            Texture2D icon = Resources.Load<Texture2D>("settings-editor-icon");
            window.titleContent = new GUIContent("Memory settings", icon);
        }

        private void CreateGUI()
        {
            MemorySettings settings = MemorySettings.Instance;

            Editor editor = Editor.CreateEditor(settings);
            IMGUIContainer container = new(editor.OnInspectorGUI);
            rootVisualElement.Add(container);
        }
    }
}
#endif