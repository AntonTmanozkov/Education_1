using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MemorySaveSystem.Editor.SavePresets
{
    public class SavePresetList : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
        [SerializeField] private VisualTreeAsset m_PresetListElement = default;

        private VisualElement _root;
        private SavePresetController _controller;
        private TextAsset[] _presets;
        private TextAsset _selectedPreset;
        public Button AddButton => _root.Q<Button>("add-button");
        public Button RemoveButton => _root.Q<Button>("remove-button");
        public Button RenameButton => _root.Q<Button>("rename-button");
        public Button ApplyButton => _root.Q<Button>("apply-button");
        public Button CreateCopyButton => _root.Q<Button>("create-copy");
        public Button LoadCurrentButton => _root.Q<Button>("load-current-button");
        public TextField NameField => _root.Q<TextField>("name-field");
        public TextField SaveField => _root.Q<TextField>("save-text");
        public ListView PresetList => _root.Q<ListView>("save-preset-list");
        public TextField SaveTextField => _root.Q<TextField>("save-text");
        public VisualElement PresetInfo => _root.Q<VisualElement>("preset-info");
        public IMGUIContainer GUIContainer => _root.Q<IMGUIContainer>("save-text");
        public TextAsset SelectedPreset => _selectedPreset;

        [MenuItem("Memory/Save presets")]
        public static void ShowExample()
        {
            SavePresetList wnd = GetWindow<SavePresetList>();
            wnd.titleContent = new GUIContent("Save presets");
        }

        public void CreateGUI()
        {
            _root = rootVisualElement;
            _root.Add(m_VisualTreeAsset.Instantiate());

            InitList();

            _controller = new(this);
        }

        private void InitList()
        {
            PresetList.makeItem = () => m_PresetListElement.Instantiate();
            PresetList.bindItem = BindPreset;
            PresetList.selectionChanged += OnItemSelect;
            OnItemSelect(new object[] {});
            UpdatePresetList();
        }

        private void BindPreset(VisualElement root, int index)
        {
            Label nameField = root.Q<Label>("name-field");
            nameField.text = _presets[index].name;
        }

        public void UpdatePresetList()
        {
            _presets = Resources.LoadAll<TextAsset>("Memory/Save presets/");

            PresetList.itemsSource = _presets;
            PresetList.RefreshItems();
            PresetList.selectedIndex = _presets.ToList().IndexOf(_selectedPreset);
        }

        private void OnItemSelect(IEnumerable<object> objects)
        {
            if (objects.Count() == 0)
            {
                PresetInfo.style.visibility = Visibility.Hidden;
                NameField.value = "";
                GUIContainer.Clear();
                return;
            }

            _selectedPreset = objects.First() as TextAsset;
            PresetInfo.style.visibility = Visibility.Visible;
            UpdateSelectedItemInfo();
        }

        public void UpdateSelectedItemInfo()
        {
            NameField.value = _selectedPreset.name;

            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(_selectedPreset);

            GUIContainer.onGUIHandler = editor.OnInspectorGUI;
        }
    }
}