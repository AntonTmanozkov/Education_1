using System;
using UnityEngine;

namespace RealTimeSystem
{
    [Serializable]
    internal class DayTimeSpan
    {
        [SerializeField] private int _hours;
        [SerializeField] private int _minutes;
        [SerializeField] private int _seconds;

        public int Hours => _hours;
        public int Minutes => _minutes;
        public int Seconds => _seconds;

        public void OnValidate()
        {
            _hours = Mathf.Clamp(_hours, 0, 23);
            _minutes = Mathf.Clamp(_minutes, 0, 59);
            _seconds = Mathf.Clamp(_seconds, 0, 59);
        }
    }
}