using UnityEngine;
using MemorySaveSystem;
using Unity.Android.Gradle.Manifest;
using System;

public class TimeSystem : MonoBehaviour
{
    [field:SerializeField] private string _saveKey = "Time_Save";

    public TimeSpan _pastTime => DateTime.UtcNow - _presentTime;
    public DateTime _presentTime { get; private set; }

    private void Awake() => Load();

    private void OnEnable() => Memory.BeforeSaved += Save;
   
    public void Load() => _presentTime = new Load<DateTime>(_saveKey);

    public void Save() => new Save(_saveKey, DateTime.UtcNow);

    private void OnDestroy() => Memory.BeforeSaved -= Save;
}
