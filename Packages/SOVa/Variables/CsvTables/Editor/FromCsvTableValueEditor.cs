using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SOVa.CsvTables
{
    public abstract class FromCsvTableValueEditor : Editor
    {
        private IValueFromCsvTable _target;
        private SerializedProperty columnProp;
        private SerializedProperty rowProp;
        private SerializedProperty tableProp;
        private VisualElement _valueElement;
        private PropertyField tablePropField;
        private VisualElement tablePropsVisualElement;

        private void FindProps()
        {
            tableProp = serializedObject.FindProperty("_table");
            rowProp = serializedObject.FindProperty("_row");
            columnProp = serializedObject.FindProperty("_column");
        }
        public override VisualElement CreateInspectorGUI()
        {
            FindProps();
            _target = (IValueFromCsvTable)target;
            
            var root = new VisualElement();

            tablePropField = new PropertyField(tableProp);
            root.Add(tablePropField);
            tablePropsVisualElement = new VisualElement();
            tablePropField.RegisterValueChangeCallback(AddProps);
            if (_target.Table is not null)
            {
                AddTableProps();
            }
            root.Add(tablePropsVisualElement);
            return root;
        }

        private void AddProps(SerializedPropertyChangeEvent evt)
        {
            if (tableProp.objectReferenceValue is null)
            {
                tablePropsVisualElement.Clear();
            }
            else if(tablePropsVisualElement.childCount == 0)
            {
                AddTableProps();
            }
        }

        private void AddTableProps()
        {
            tablePropsVisualElement.Add(FromCSvTableEditorTools.CreateDropdown("Row", _target.Table.Rows, rowProp, OnRowDropdownChange));
            tablePropsVisualElement.Add(FromCSvTableEditorTools.CreateDropdown("Column", _target.Table.Columns, columnProp, OnColumnDropdownChange));
            _valueElement = new VisualElement();
            FromCSvTableEditorTools.UpdateValueElement(_target, _valueElement);
            tablePropsVisualElement.Add(_valueElement);
        }

        private void OnColumnDropdownChange(ChangeEvent<string> evt)
        {
            columnProp.stringValue = evt.newValue;
            serializedObject.ApplyModifiedProperties();
            FromCSvTableEditorTools.UpdateValueElement(_target, _valueElement);
        }
        private void OnRowDropdownChange(ChangeEvent<string> evt)
        {
            rowProp.stringValue = evt.newValue;
            serializedObject.ApplyModifiedProperties();
            FromCSvTableEditorTools.UpdateValueElement(_target, _valueElement);
        }

        public override void OnInspectorGUI()
        {
            FindProps();

            EditorGUILayout.PropertyField(tableProp);
            EditorGUILayout.PropertyField(rowProp);
            EditorGUILayout.PropertyField(columnProp);


            serializedObject.ApplyModifiedProperties();
                
            EditorGUILayout.LabelField($"Value: {_target.ValueObj}");
        }
    }
}