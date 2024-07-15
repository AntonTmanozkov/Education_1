using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Profiling;
#endif

namespace RealTimeSystem
{
    [Serializable]
    internal class TimeSpanWrapper
    {
        [SerializeField] private int _days;
        [SerializeField] private int _hours;
        [SerializeField] private int _minutes;
        [SerializeField] private int _seconds;

        public int Days => _days;
        public int Hours => _hours;
        public int Minutes => _minutes;
        public int Seconds => _seconds;
        public long SumOfTicks() =>
                 SumOfSeconds() * TimeSpan.TicksPerSecond;
        public int SumOfSeconds() =>
            _seconds +
                _minutes * 60 +
                _hours * 3600 +
                _days * 3600 * 24;

#if UNITY_EDITOR

        public void OnValidate()
        {
            _days = Mathf.Clamp(_days, 0, int.MaxValue);
            _hours = Mathf.Clamp(_hours, 0, 23);
            _minutes = Mathf.Clamp(_minutes, 0, 59);
            _seconds = Mathf.Clamp(_seconds, 0, 59);
        }

#endif
    }
}