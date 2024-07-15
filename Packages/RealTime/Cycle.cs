using MemorySaveSystem;
using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityExtentions;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace RealTimeSystem
{
    [CreateAssetMenu(menuName = "RealTimeSystem/Cycle")]
    public class Cycle : ScriptableObject, INeedInit
    {
        private const string SAVE_PRFIX = "CYCLE_SAVE:";

        [Header("Do not forget to put to the Resources foulder")]
        [SerializeField] private float _secondsInterval;

        [SerializeField] private float _minutesInterval;
        [SerializeField] private float _hoursInterval;
        [SerializeField] private float _daysInterval;
        [SerializeField] private bool _workInRealTime = true;
        [Tooltip("If not used, selected a user's current time, " +
            "when there is no save")]
        [SerializeField] private bool _useDefault;

        [Header("Time must be in UTC")]
        [SerializeField] private DayTimeSpan _defaultTime;

        private float _cycleInterval;

        private DateTime _lastTrigger;

        public event UnityAction<TriggeredData> Triggered;

        /// <summary>
        /// How much time passed since the last trigger.
        /// </summary>
        public TimeSpan TimePassed { get; private set; }

        public TimeSpan TimeLeft { get; private set; }
        private string SaveKey => SAVE_PRFIX + this.name;

        RuntimeInitializeLoadType[] INeedInit.When => new[] { RuntimeInitializeLoadType.AfterSceneLoad };

        void INeedInit.Init(RuntimeInitializeLoadType time)
        {
            _cycleInterval =
                _secondsInterval +
                _minutesInterval * 60 +
                _hoursInterval * 3600 +
                _daysInterval * 3600 * 24;

            DoLoad();
            Memory.BeforeSaved += DoSave;
            Update();
        }

        private async void Update()
        {
            int interval = 1000;

            while (Application.isPlaying)
            {
                CheckTriggerTime();
                await Task.Delay(interval);
            }
        }

        private void CheckTriggerTime()
        {
            TimePassed = (DateTime.UtcNow - _lastTrigger);
            TimeLeft = new TimeSpan((long)(_cycleInterval - TimePassed.TotalSeconds) * TimeSpan.TicksPerSecond);
            if (TimePassed.TotalSeconds > _cycleInterval)
            {
                TriggeredData data = new()
                {
                    Count = (int)(TimePassed.TotalSeconds / _cycleInterval),
                };
                _lastTrigger = DateTime.UtcNow;
                Triggered?.Invoke(data);
            }
        }

        private void DoSave()
        {
            if (_workInRealTime == false) return;

            SaveData data = new()
            {
                LastTrigger = _lastTrigger.ToString(RealTime.DATE_FORMAT),
            };
            new Save(SaveKey, data);
        }

        private void DoLoad()
        {
            if (_workInRealTime == false)
            {
                _lastTrigger = DateTime.UtcNow;
                CheckTriggerTime();
                return;
            }

            SaveData data = new Load<SaveData>(SaveKey);
            if (data == null)
            {
                if (_useDefault)
                {
                    _lastTrigger = DayTimeSpanTools.GetDefault(_defaultTime);
                }
                else
                {
                    _lastTrigger = DateTime.UtcNow;
                }
            }
            else
            {
                _lastTrigger = DateTime.ParseExact(data.LastTrigger, RealTime.DATE_FORMAT, CultureInfo.CurrentCulture);
            }

            CheckTriggerTime();
        }

        public struct TriggeredData
        {
            public int Count;
        }

        private class SaveData
        {
            public string LastTrigger;
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(Cycle))]
        private class CycleEditor : Editor
        {
            private Cycle _target;

            private void Awake()
            {
                _target = (Cycle)target;
            }

            public override void OnInspectorGUI()
            {
                SerializedProperty secondsProp = serializedObject.FindProperty("_secondsInterval");
                SerializedProperty minutesProp = serializedObject.FindProperty("_minutesInterval");
                SerializedProperty hoursProp = serializedObject.FindProperty("_hoursInterval");
                SerializedProperty daysProp = serializedObject.FindProperty("_daysInterval");
                SerializedProperty workInRealTimeProp = serializedObject.FindProperty("_workInRealTime");
                SerializedProperty useDefaultProp = serializedObject.FindProperty("_useDefault");
                SerializedProperty defaultTimeProp = serializedObject.FindProperty("_defaultTime");
                EditorGUILayout.PropertyField(secondsProp);
                EditorGUILayout.PropertyField(minutesProp);
                EditorGUILayout.PropertyField(hoursProp);
                EditorGUILayout.PropertyField(daysProp);
                EditorGUILayout.PropertyField(workInRealTimeProp);
                EditorGUILayout.PropertyField(useDefaultProp);
                if (useDefaultProp.boolValue == true)
                {
                    EditorGUILayout.PropertyField(defaultTimeProp);
                }
                if (Application.isPlaying)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Last time triggered UTC: {_target._lastTrigger}");
                    EditorGUILayout.LabelField($"Time passed: {_target.TimePassed}");
                    EditorGUILayout.LabelField($"Time left: {_target.TimeLeft}");
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

#endif
    }
}