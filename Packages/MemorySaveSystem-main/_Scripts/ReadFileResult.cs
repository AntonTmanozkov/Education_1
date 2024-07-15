namespace MemorySaveSystem
{
    public struct ReadFileResult
    {
        public FileSource SaveSource;
        public State ReadResult;
        internal MemoryFile<string, string> Save;
    }

    public struct WriteFileResult 
    {
        public FileSource SaveSource;
        public State WriteResult;
    }

    public enum State
    {
        Success,
        Failed
    }

    public enum FileSource 
    {
        Device,
#if UNITY_CLOUD_SAVE
        UnityServices
#endif
    }
}
