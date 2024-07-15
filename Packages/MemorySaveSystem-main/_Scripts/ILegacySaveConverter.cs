using UnityEngine;

namespace MemorySaveSystem
{
    public interface ILegacySaveConverter
    {
        public string[] Keys { get; }
        public string[] VersionsToConvert { get; }
        public void Convert(string legacySave, string key);
    }
    
    public abstract class ILegacySaveConverter<ILegacySaveSerealization> : ILegacySaveConverter
    {
        public abstract string[] Keys { get; }
        public abstract string[] VersionsToConvert { get; }

        public void Convert(string legacySave, string key) => Convert(JsonUtility.FromJson<ILegacySaveSerealization>(legacySave), key);
        public abstract void Convert(ILegacySaveSerealization serealization, string key);
    }
}
