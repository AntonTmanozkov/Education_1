using System.Collections.Generic;
using UnityEngine;

namespace SOVa.CsvTables
{
    public abstract class CsvTableBase : ScriptableObject
    {
        public abstract IEnumerable<string> Columns { get; }
        public abstract IEnumerable<string> Rows { get; }
        public abstract string GetValue(string row, string column);
    }
}