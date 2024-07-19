using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
#endif

namespace SOVa.CsvTables 
{ 
    [CreateAssetMenu(menuName = "Values/CSV/Int")]
    public class IntFromCSV : Int
    {
        [SerializeField] private TSVTableValue<int> _table;

        public override int Value
        {
            get
            {
                return _table.Value;
            }
            set => Debug.LogError($"{nameof(IntFromCSV)} is read only value!"); 
        }

        protected override void Load()
        {
            base.Load();
            if (_table.ValueIsReaded == false)
            {
                _table.ReadValueFromTable();
            }
            if (LogChanges) 
            {
                Debug.Log($"Readed value from csv: {Value}");
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            EditorApplication.delayCall += _OnValidate;
        }

        private void _OnValidate()
        {
            EditorApplication.delayCall -= _OnValidate;
            
            if (_table.Table != null)
            {
                _table.ReadValueFromTable();
            }
        }

        [CustomEditor(typeof(IntFromCSV)), CanEditMultipleObjects]
        private class CEditor : Editor 
        {
            public override VisualElement CreateInspectorGUI()
            {
                SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
                SerializedProperty tableProperty = serializedObject.FindProperty("_table");
                SerializedProperty debugProperty = serializedObject.FindProperty("_debug");

                VisualElement result = new();

                CSVTableValueDrawer drawer = new();
                VisualElement scriptField = new PropertyField(scriptProperty);
                scriptField.SetEnabled(false);
                VisualElement tableVisualElement = drawer.CreatePropertyGUI(tableProperty);
                VisualElement debugElement = new PropertyField(debugProperty);
                result.Add(scriptField);
                result.Add(tableVisualElement);
                result.Add(debugElement);
                return result;
            }
        }
#endif
    }
}
