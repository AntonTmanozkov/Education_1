using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace SOVa.CsvTables
{
    [CreateAssetMenu(menuName = "SOVa/Tables/TSVTable")]
    public class CsvTable : CsvTableBase
    {
        [SerializeField] TextAsset _table;
        private bool _readen;
        private string[] _rowNames = Array.Empty<string>();
        private string[] _columnNames = Array.Empty<string>();
        private List<string>[] _valuesArray = Array.Empty<List<string>>();

        private void OnEnable()
        {
            if (_readen || _table == null)
            {
                return;
            }
            
            ReadFromTable();
        }

        public override IEnumerable<string> Columns => Array.AsReadOnly(_columnNames);
        public override IEnumerable<string> Rows => Array.AsReadOnly(_rowNames);

        public override string GetValue(string row, string column)
        {
#if UNITY_EDITOR
            if (_table == null)
            {
                throw new Exception($"Table {name} is not yet initialized!");
            }

            if (_readen == false)
            {
                ReadFromTable();
            }
            
            if (_rowNames.Contains(row) == false)
            {
                throw new Exception($"You passed row {row} that doesn't exists in {name}");
            }

            if (_columnNames.Contains(column) == false)
            {
                throw new Exception($"You passed column {column} that doesn't exists in {name}");
            }
#endif

            
            int rowIndex = Array.IndexOf(_rowNames, row);
            int columnIndex = Array.IndexOf(_columnNames, column)+1;
            return _valuesArray[rowIndex][columnIndex];
        }

        public T GetValue<T>(string row, string column) 
        {
            string value = GetValue(row, column);
            Type type = typeof(T);
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return (T)converter.ConvertFrom(value);
        }

        public void ReadFromTable() 
        {
            if (_table == null) 
            {
                return;
            }
            string text = _table.text;

            string[] rows = text.Split(TSVTableValue.LINE_SEPARATOR);
            List<List<string>> valuesList = new();
            for (int i = 0; i < rows.Length; i++) 
            {
                List<string> rowValues = rows[i].Split(TSVTableValue.SEPARATOR).ToList();
                valuesList.Add(rowValues);
            }

            _valuesArray = valuesList.ToArray();
            _rowNames = GetRowNames(text);
            _columnNames = GetColumnNames(text);

            _readen = true;
        }

        private string[] GetRowNames(string text)
        {
            string[] textInRows = text.Split('\n');
            List<string> result = new();
            foreach (string i in textInRows)
            {
                string name = i.Split(TSVTableValue.SEPARATOR)[0];
                result.Add(name);
            }

            return result.ToArray();
        }

        private string[] GetColumnNames(string text)
        {
            List<string> textInRows = text.Split('\n').ToList();
            List<string> result = textInRows[0].Split(TSVTableValue.SEPARATOR).ToList();
            result.RemoveAt(0);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = Regex.Replace(result[i], @"\s+", "");
            }
            return result.ToArray();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ReadFromTable();
        }

        [CustomEditor(typeof(CsvTable))]
        private class TSVTableEditor : Editor
        {
            private CsvTable _target;

            private void Awake()
            {
                _target = (CsvTable)target;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (GUILayout.Button("Refresh"))
                {
                    _target.ReadFromTable();
                }
            }
        }
#endif
    }
}