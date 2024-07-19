using System.Linq;
using UnityEngine;

namespace SOVa.CsvTables
{
    [CreateAssetMenu(menuName = "SOVa/Tables/Level Int")]
    public class LevelIntFromCsvTable : Int, IValueFromCsvTable
    {
        [SerializeField] private CsvTableBase _table;
        [SerializeField] private string _column;
        [SerializeField] private Reference<int> _row = new Reference<int>(1);
        [SerializeField] private int _indexOffset;
        

        public override bool HaveOldValue => false;
        public Reference<int> RowReference => _row;
        public string Column => _column;

        #region IValueFromCsv
        public CsvTableBase Table => _table;
        public bool IsColumnEmpty => string.IsNullOrEmpty(_column);
        public bool IsRowConstant => _row.IsConstant;
        public bool IsRowHasReference => _row.HaveReference;
        object IValueFromCsvTable.ValueObj => Value;
        string IValueFromCsvTable.TableValue => TableValue;
        public string TypeName => "Int";
        bool IValueFromCsvTable.TryGetValue(out object value)
        {
            var parsed = int.TryParse(TableValue, out int result);
            if (parsed)
            {
                value = result;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        #endregion

        protected override void Load()
        {
            if (_row.HaveReference && _row.IsConstant == false)
            {
                _row.Variable.Changed.AddListener(InvokeChanged);
            }
            else if (_row.IsConstant)
            {
                _row.Changed += InvokeChanged;
            }
        }

        public override int Value
        {
            get
            {
                var parsed = int.TryParse(TableValue, out int result);
                if (parsed)
                {
                    return result;
                }
                else
                {
                    Debug.LogError($"Couldn't convert {name} to int!", this);
                    return 0;
                }
            
            }
            set => UnityEngine.Debug.LogError($"You cannot set value to {name}", this);
        }

        private string TableValue
        {
            get
            {
                if (_row.HaveReference == false && _row.IsConstant == false)
                {
                    Debug.LogError("Row reference value is null!", this);

                    // First element (at 0 index) is usually empty
                    // so we take first not empty element
                    var firstRow = _table.Rows.ToArray()[1];
                    
                    return _table.GetValue(firstRow, _column);
                }

                if (_row.IsConstant)
                {
                    return _table.GetValue((_row.Value).ToString(), _column);
                }
                else
                {
                    return _table.GetValue((_row.Value + _indexOffset).ToString(), _column);
                }
            }
        }
    }
}