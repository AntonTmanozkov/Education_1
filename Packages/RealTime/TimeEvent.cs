using MemorySaveSystem;
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityExtentions;

namespace RealTimeSystem
{
    [CreateAssetMenu(menuName = "RealTimeSystem/TimeEvent")]
    public class TimeEvent : ScriptableObject, INeedInit
    {
        private const string SAVE_PREFIX = "TIME_EVENT:";

        [SerializeField] private TimeSpanWrapper _duration;
        [SerializeField] private bool _useSave;
        [SerializeField] private bool _useDefault;
        [SerializeField] private DayTimeSpan _defaultTime;
        [SerializeField] private string _saveName;

        private DateTime _startedTime;
        public delegate void TimerBeginedHandler();
        public event TimerBeginedHandler TimerBegined;

        public bool IsEnded => _startedTime != default && TimeLeft.TotalSeconds < 0;
        public bool IsTicking => TimeLeft.TotalSeconds > 0;
        public TimeSpan TimePassed => DateTime.UtcNow - _startedTime;
        public TimeSpan TimeLeft => EndTime - DateTime.UtcNow;
        public TimeSpan FullTime => new TimeSpan(_duration.SumOfTicks());
        public DateTime EndTime { get; private set; }
        public DateTime StartedTime => _startedTime;
        private string SaveKey => SAVE_PREFIX + _saveName;

        RuntimeInitializeLoadType[] INeedInit.When => new[] { RuntimeInitializeLoadType.BeforeSceneLoad };

        public static TimeEvent CreateCopy(TimeEvent original, string saveName) =>
            ScriptableObject.Instantiate(original).Init(saveName);
        public static TimeEvent CreateCopy(TimeEvent original) =>
            ScriptableObject.Instantiate(original).Init();
        private TimeEvent Init()
        {
            if(_useSave)
            {
                SubscribeToSave();
            }

            return this;
        }
        private TimeEvent Init(string saveName)
        {
            if (string.IsNullOrEmpty(saveName) == false)
            {
                _saveName = saveName;
                SubscribeToSave();
            }
            else
            {
                Debug.LogError(new ArgumentException("Save name is null or empty!"), this);
            }

            return this;
        }

        public void BeginEvent(bool force = false)
        {
            if (_startedTime != default && force || _startedTime == default)
            {
                if(_useDefault)
                {
                    _startedTime = DayTimeSpanTools.GetDefault(_defaultTime);
                }
                else
                {
                    _startedTime = DateTime.UtcNow;
                }
                EndTime = new DateTime(_startedTime.Ticks + _duration.SumOfTicks(), DateTimeKind.Utc);
                TimerBegined?.Invoke();
            }
        }

        public void SkipTime(TimeSpan time)
        {
            EndTime = new DateTime(EndTime.Ticks - time.Ticks, DateTimeKind.Utc);
        }

        private void DoSave()
        {
            SaveData data = new()
            {
                Started = _startedTime.ToString(RealTime.DATE_FORMAT),
            };
            new Save(SaveKey, data);
        }

        private void DoLoad()
        {
            SaveData data = new Load<SaveData>(SaveKey);
            if (data != null)
            {
                _startedTime = DateTime.ParseExact(data.Started, RealTime.DATE_FORMAT, CultureInfo.InvariantCulture);
                EndTime = new DateTime(_startedTime.Ticks + _duration.SumOfTicks(), DateTimeKind.Utc);
            }
        }

        private void SubscribeToSave()
        {
            DoLoad();
            Memory.BeforeSaved += DoSave;
            Memory.Loaded += DoLoad;
        }

        void INeedInit.Init(RuntimeInitializeLoadType time)
        {
            if (_useSave)
            {
                SubscribeToSave();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _duration.OnValidate();
        }
#endif

        [Serializable]
        private class SaveData
        {
            public string Started;
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(TimeEvent))]
        private class TimeEventEditor : Editor
        {
            private TimeEvent _target;

            private void Awake()
            {
                _target = (TimeEvent)target;
            }

            public override void OnInspectorGUI()
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                GUI.enabled = true;

                var durationProp = serializedObject.FindProperty("_duration");
                EditorGUILayout.PropertyField(durationProp);

                var useDefaultProp = serializedObject.FindProperty("_useDefault");
                EditorGUILayout.PropertyField(useDefaultProp);


                if (useDefaultProp.boolValue == true)
                {
                    EditorGUILayout.LabelField("Specify time in UTC!", EditorStyles.boldLabel);
                    var defaultTimeProp = serializedObject.FindProperty("_defaultTime");
                    EditorGUILayout.PropertyField(defaultTimeProp);
                    EditorGUILayout.Space();
                }

                var useSaveProp = serializedObject.FindProperty("_useSave");
                EditorGUILayout.PropertyField(useSaveProp);
                if (useSaveProp.boolValue == true)
                {
                    var saveNameProp = serializedObject.FindProperty("_saveName");
                    EditorGUILayout.PropertyField(saveNameProp);
                }
                if (Application.isPlaying)
                {
                    if (_target.StartedTime == default)
                    {
                        if (GUILayout.Button("Start"))
                        {
                            _target.BeginEvent();
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Started: {_target.StartedTime}");
                        EditorGUILayout.LabelField($"Time passed: {_target.TimePassed}");
                        EditorGUILayout.LabelField($"Time left: {_target.TimeLeft}");
                        EditorGUILayout.LabelField($"End at: {_target.EndTime}");
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

#endif
    }
}