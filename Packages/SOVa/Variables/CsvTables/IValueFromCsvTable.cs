namespace SOVa.CsvTables
{
    public interface IValueFromCsvTable
    {
        public CsvTableBase Table { get; }
        public bool IsColumnEmpty { get; }
        public bool IsRowConstant { get; }
        public bool IsRowHasReference { get; }
        public object ValueObj { get; }
        public string TableValue { get; }
        public bool TryGetValue(out object value);
        public string TypeName { get; }
    }
}