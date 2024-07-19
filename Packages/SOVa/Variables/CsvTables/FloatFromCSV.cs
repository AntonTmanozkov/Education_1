using SOVa.CommonVariables;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace SOVa.CsvTables
{
    [CreateAssetMenu(menuName = "Values/CSV/Float")]
    public class FloatFromCSV : Float
    {
        [SerializeField] private TSVTableValue<float> _table;

        public override float Value
        {
            get
            {
                return _table.Value;
            }
            set => UnityEngine.Debug.LogError($"{nameof(IntFromCSV)} is read only value!");
        }

        protected override void Load()
        {
            if (_table.ValueIsReaded == false)
            {
                _table.ReadValueFromTable();
            }
            if (LogChanges)
            {
                UnityEngine.Debug.Log($"[{name}]: Readed value from csv: {Value}", this);
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
        [CustomEditor(typeof(FloatFromCSV)), CanEditMultipleObjects]
        private class CEditor : Editor
        {
            public override VisualElement CreateInspectorGUI()
            {
                SerializedProperty tableProperty = serializedObject.FindProperty("_table");
                SerializedProperty debugProperty = serializedObject.FindProperty("_debug");

                VisualElement result = new();

                CSVTableValueDrawer drawer = new();
                VisualElement tableVisualElement = drawer.CreatePropertyGUI(tableProperty);
                VisualElement debugElement = new PropertyField(debugProperty);
                result.Add(tableVisualElement);
                result.Add(debugElement);
                return result;
            }
        }
#endif
    }
}
