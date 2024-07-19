using UnityEngine;

namespace SOVa.CsvTables
{
    [CreateAssetMenu(menuName = "SOVa/Tables/Int")]
    public class IntFromCsvTable : Int, IValueFromCsvTable
    {
        [SerializeField] private CsvTableBase _table;
        [SerializeField] private string _column;
        [SerializeField] private string _row;

        public override bool HaveOldValue => false;

        #region IValueFromCsv

        CsvTableBase IValueFromCsvTable.Table => _table;
        public bool IsColumnEmpty => false;
        public bool IsRowConstant => true;
        public bool IsRowHasReference => false;
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

        private string TableValue => _table.GetValue(_row, _column);
    }
}