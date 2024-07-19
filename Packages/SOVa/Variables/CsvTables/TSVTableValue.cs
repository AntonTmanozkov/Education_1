using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace SOVa.CsvTables
{
    public class TSVTableValue
    {
        public const char SEPARATOR = '\t';
        public const char LINE_SEPARATOR = '\n';
    }

    [Serializable]
    public abstract class TSVTableValueBase
    {
        public abstract void TryReadValueFromTable();
    }
    [Serializable]
    public class TSVTableValue<T>
    {

        [SerializeField] TextAsset _table;
        [SerializeField] string _rowName;
        [SerializeField] string _columnName;
        [SerializeField] T _recordedValue;
        private bool _valueIsReaded;

        public TextAsset Table => _table;
        public bool ValueIsReaded => _valueIsReaded;
        public override string ToString()
        {
            return _recordedValue.ToString();
        }

        public T Value 
        {
            get 
            {
                if (_valueIsReaded == false) 
                {
                    ReadValueFromTable();
                }
                return _recordedValue;
            }
        }

        public void TryReadValueFromTable()
        {
            if (_valueIsReaded == false) 
            {
                ReadValueFromTable();
            }
        }
        public void ReadValueFromTable() 
        {
            string text = _table.text;

            string[] rows = text.Split(TSVTableValue.LINE_SEPARATOR);
            List<List<string>> valuesArray = new();
            for (int i = 0; i < rows.Length; i++) 
            {
                List<string> rowValues = rows[i].Split(TSVTableValue.SEPARATOR).ToList();
                valuesArray.Add(rowValues);
            }

            List<string> rowNames = GetRowNames(text);
            List<string> columnNames = GetColumnNames(text);

            int rowIndex = rowNames.IndexOf(_rowName);
            int columnIndex = columnNames.IndexOf(_columnName)+1;

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            string toConvert = valuesArray[rowIndex][columnIndex];
            try 
            {
                T result;
                if (typeof(T) == typeof(float)) 
                {
                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";

                    result = (T)(object)float.Parse(toConvert, NumberStyles.Float, ci);
                }
                else 
                {
                    result = (T)converter.ConvertFromString(toConvert);
                }

                _recordedValue = result;
                _valueIsReaded = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't convert {toConvert} to {typeof(T).Name}. Resoun: {e}\nRow name: {_rowName}\nColumn name: {_columnName}");
                _recordedValue = default;
                _valueIsReaded = true;
            }

        }

        public List<string> GetRowNames(string text)
        {
            string[] textInRows = text.Split('\n');
            List<string> result = new();
            foreach (string i in textInRows)
            {
                string name = i.Split(TSVTableValue.SEPARATOR)[0];
                result.Add(name);
            }

            return result;
        }

        public List<string> GetColumnNames(string text)
        {
            List<string> textInRows = text.Split('\n').ToList();
            List<string> result = textInRows[0].Split(TSVTableValue.SEPARATOR).ToList();
            result.RemoveAt(0);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = Regex.Replace(result[i], @"\s+", "");
            }
            return result;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TSVTableValue<>))]
    public class CSVTableValueDrawer : PropertyDrawer
    {
        private SerializedProperty serializedTableProperty;
        private SerializedProperty serializedRowNameProperty;
        private SerializedProperty mainProperty;
        private SerializedProperty serializedColumnNameProperty;
        private SerializedProperty recordedValueProperty;
        private VisualElement root;
        private PopupField<string> selectedRowPopup;
        private PopupField<string> selectedColumnPopup;
        private Label valueField;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUI.PropertyField(new Rect(position), property);
            EditorGUI.BeginProperty(position, label, property);
            
            // Don't make child fields be indented
            // var indent = EditorGUI.indentLevel;
            // EditorGUI.indentLevel = 0;

            // Calculate rects
            var tableRect = new Rect(position.x, position.y, 70, position.height);
            var rowRect = new Rect(position.x + 75, position.y, 80, position.height);
            var columnRect = new Rect(position.x + 160, position.y, 80, position.height);
            var valueRect = new Rect(position.x + 250, position.y, 60, position.height);
            //
            serializedTableProperty = property.FindPropertyRelative("_table");
            serializedRowNameProperty = property.FindPropertyRelative("_rowName");
            serializedColumnNameProperty = property.FindPropertyRelative("_columnName");
            recordedValueProperty = property.FindPropertyRelative("_recordedValue");
            
            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(tableRect, serializedTableProperty, GUIContent.none);
            EditorGUI.PropertyField(rowRect, serializedRowNameProperty, GUIContent.none);
            EditorGUI.PropertyField(columnRect, serializedColumnNameProperty, GUIContent.none);
            EditorGUI.PropertyField(valueRect, recordedValueProperty, GUIContent.none);

            // Set indent back to what it was
            // EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty(); 
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            root = new();
            mainProperty = property;
            serializedTableProperty = property.FindPropertyRelative("_table");
            serializedRowNameProperty = property.FindPropertyRelative("_rowName");
            serializedColumnNameProperty = property.FindPropertyRelative("_columnName");
            recordedValueProperty = property.FindPropertyRelative("_recordedValue");

            PropertyField tableProperty = new(serializedTableProperty);
            tableProperty.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnTableChange);

            PropertyField recordedValueFiled = new(recordedValueProperty);
            recordedValueFiled.SetEnabled(false);

            valueField = new();

            root.Add(tableProperty);
            root.Add(valueField);
            root.Add(recordedValueFiled);

            return root;
        }

        public string OnRowChange(string newValue)
        {
            serializedRowNameProperty.stringValue = newValue;

            mainProperty.serializedObject.ApplyModifiedProperties();
            return newValue;
        }

        public string OnColumnChange(string newValue)
        {
            serializedColumnNameProperty.stringValue = newValue;

            mainProperty.serializedObject.ApplyModifiedProperties();
            return newValue;
        }

        private void OnTableChange(ChangeEvent<UnityEngine.Object> arg)
        {
            UpdateAddictionalPropertysState();
        }

        private void UpdateAddictionalPropertysState()
        {
            if (selectedRowPopup != null)
            {
                root.Remove(selectedRowPopup);
                root.Remove(selectedColumnPopup);
                selectedRowPopup = null;
                selectedColumnPopup = null;
            }

            if (serializedTableProperty.objectReferenceValue != null)
            {
                string selectedRowName = serializedRowNameProperty.stringValue;
                string selectodColumnName = serializedColumnNameProperty.stringValue;
                string fileText = (serializedTableProperty.objectReferenceValue as TextAsset).text;

                List<string> rowNames = GetRowNames(fileText);
                int index = rowNames.IndexOf(selectedRowName);
                if (index == -1) index = 0;
                selectedRowPopup = new("Row name", rowNames, index, OnRowChange);
                root.Add(selectedRowPopup);

                List<string> columnNames = GetColumnNames(fileText);
                index = columnNames.IndexOf(selectodColumnName);
                if (index == -1) index = 0;
                selectedColumnPopup = new("Column name", columnNames, index, OnColumnChange);
                root.Add(selectedColumnPopup);
            }
        }

        public List<string> GetRowNames(string text)
        {
            string[] textInRows = text.Split('\n');
            List<string> result = new();
            foreach (string i in textInRows)
            {
                string name = i.Split(TSVTableValue.SEPARATOR)[0];
                result.Add(name);
            }

            return result;
        }

        public List<string> GetColumnNames(string text)
        {
            List<string> textInRows = text.Split('\n').ToList();
            List<string> result = textInRows[0].Split(TSVTableValue.SEPARATOR).ToList();
            result.RemoveAt(0);
            for (int i = 0; i < result.Count; i++) 
            {
                result[i] = Regex.Replace(result[i], @"\s+", "");
            }
            return result;
        }
    }
#endif
}
