using MemorySaveSystem;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace RealTimeSystem
{
    public class RealTime : MonoBehaviour
    {
        private const string SAVE_KEY = "LAST_LOGIN";
        private const string OBJECT_NAME = "-L Real time calculator";
        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";
        public static CultureInfo Culture => new CultureInfo("");
        private static RealTime _instance;
        private DateTime _pauseTime;
        private bool _isStarted;
        public static RealTime Instance => _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            GameObject gameobject = new(OBJECT_NAME);

            _instance = gameobject.AddComponent<RealTime>();
            DontDestroyOnLoad(gameobject);  
        }

        [SerializeField] private float _secondsPassed;
        [SerializeField] private UnityEvent<float> _timePassed = new(); 
        public float SecondsPassed => _secondsPassed;
        public event UnityAction<float> TimePassed
        {
            add => _timePassed.AddListener(value);
            remove => _timePassed.RemoveListener(value);
        }

        private void Awake()
        {
            Load();
        }

        private void Start()
        {
            _timePassed.Invoke(_secondsPassed);
            Memory.BeforeSaved += DoSave;
            _isStarted = true;
        }

        private void Load()
        {
            DateTime lastLogin;
            if (Memory.Contains(SAVE_KEY))
            {
                SaveData data = new Load<SaveData>(SAVE_KEY);
                lastLogin = DateTime.ParseExact(data.DateTime, DATE_FORMAT, CultureInfo.CurrentCulture);
            }
            else
            {
                lastLogin = DateTime.UtcNow;
            }
            _secondsPassed = (float)(DateTime.UtcNow - lastLogin).TotalSeconds;
        }

        private void DoSave()
        {
            SaveData data = new()
            {
                DateTime = DateTime.UtcNow.ToString(DATE_FORMAT),
            };
            new Save(SAVE_KEY, data);
        }

        private void OnApplicationPause(bool pause)
        {
            if (_isStarted == false) return;

            if (pause)
            {
                _pauseTime = DateTime.UtcNow;
            }
            else
            {
                _secondsPassed = (float)(DateTime.UtcNow - _pauseTime).TotalSeconds;
                _timePassed.Invoke(_secondsPassed);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public string DateTime;
        }
    }
}