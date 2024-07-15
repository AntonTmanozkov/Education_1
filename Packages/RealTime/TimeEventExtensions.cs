namespace RealTimeSystem
{
    public static class TimeEventExtensions
    {
        public static TimeEvent CreateCopy(this TimeEvent original, string saveName) =>
            TimeEvent.CreateCopy(original, saveName);
        public static TimeEvent CreateCopy(this TimeEvent original) =>
            TimeEvent.CreateCopy(original);
    }
}