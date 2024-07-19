namespace SOVa.CommonVariables
{
    public interface IParametr<T> : IParametr
    {
        public new T this[int index] { get; set; }
    }

    public interface IParametr 
    {
        public int CurrentIndex { get; }
        public object this[int index] { get; set; }
    }
}
