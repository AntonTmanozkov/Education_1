using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SOVa.CsvTables
{
    public static class FromCSvTableEditorTools
    {
        public static void UpdateValueElement(IValueFromCsvTable target, VisualElement valueElement)
        {
            valueElement.Clear();
            string message;
            if (target.IsColumnEmpty)
            {
                message = $"<color=red>Error:</color> column is not set!";
            }
            else
            {
                var converted = target.TryGetValue(out object value);
                if (converted)
                {
                    message = $"Value: {value}";
                }
                else
                {
                    string tableValue = string.IsNullOrEmpty(target.TableValue) ? "Empty" : target.TableValue;
                    message = $"<color=red>Error:</color> cannot convert value to {target.TypeName}!\n" +
                              $"Table value is {tableValue}";
                }
            }
                
            valueElement.Add(new Label(message));
        }
        public static VisualElement CreateDropdown(string name, IEnumerable<string> choices, SerializedProperty prop, EventCallback<ChangeEvent<string>> act)
        {
            var dropdown = new DropdownField();
            dropdown.label = name;
            dropdown.choices.AddRange(choices);
            if (string.IsNullOrEmpty(prop.stringValue) == false)
            {
                dropdown.value = prop.stringValue;
            }

            dropdown.RegisterValueChangedCallback(act);
            return dropdown;
        }
    }
    public abstract class FromCsvTableLevelValueEditor : Editor
    {
        private IValueFromCsvTable _target;
        private SerializedProperty scriptProp;
        private SerializedProperty columnProp;
        private SerializedProperty rowProp;
        private SerializedProperty tableProp;
        private SerializedProperty indexOffsetProp;
        private VisualElement _valueElement;
        private PropertyField _rowPropertyField;
        private VisualElement _indexOffsetPropertyField;

        private void FindProps()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            tableProp = serializedObject.FindProperty("_table");
            rowProp = serializedObject.FindProperty("_row");
            columnProp = serializedObject.FindProperty("_column");
            indexOffsetProp = serializedObject.FindProperty("_indexOffset");
        }
        public override VisualElement CreateInspectorGUI()
        {
            FindProps();
            _target = (IValueFromCsvTable)target;
            
            var root = new VisualElement();
            PropertyField field = new PropertyField(scriptProp);
            field.SetEnabled(false);
            root.Add(field);
            root.Add(new PropertyField(tableProp));
            _rowPropertyField = new PropertyField(rowProp);
            _indexOffsetPropertyField = new PropertyField(indexOffsetProp);
            _rowPropertyField.RegisterValueChangeCallback(CheckRow);
            _valueElement = new VisualElement();
            root.Add(_rowPropertyField);
            if (_target.Table != null)
            {
                root.Add(FromCSvTableEditorTools.CreateDropdown("Column", _target.Table.Columns, columnProp, OnColumnDropdownChange));
                CheckRow();
                root.Add(_valueElement);
                root.Add(_indexOffsetPropertyField);
            }
            return root;
        }


        private void OnColumnDropdownChange(ChangeEvent<string> evt)
        {
            columnProp.stringValue = evt.newValue;
            serializedObject.ApplyModifiedProperties();
            CheckRow();
        }

        private void CheckRow(SerializedPropertyChangeEvent evt)
        {
            CheckRow();
        }
        private void CheckRow()
        {
            if (indexOffsetProp != null && _target.IsRowConstant == false && _target.IsRowHasReference)
            {
                _indexOffsetPropertyField.SetEnabled(true);
            }
            else
            {
                _indexOffsetPropertyField.SetEnabled(false);
            }
            
            if (_target.IsRowConstant == false && _target.IsRowHasReference == false)
            {
                return;
            }
            
            FromCSvTableEditorTools.UpdateValueElement(_target, _valueElement);
        }
    }
}