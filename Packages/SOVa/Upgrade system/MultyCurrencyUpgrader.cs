using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using SOVa;
using SOVa.CommonVariables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOVa.UpgradeSystem
{
    [CreateAssetMenu(menuName = "Game/Economic/Multycurrency upgrader")]
    public class MultyCurrencyUpgrader : Upgrader
    {
        public Int _level;
        [SerializeField] int _maxLevel = 20;
        [SerializedDictionary("Currency", "Cost")]
        [SerializeField] SerializedDictionary<Currency, Int> _costForCurrency;
        [SerializedDictionary("Int", "Cost")]
        [SerializeField] SerializedDictionary<Int, Int> _intToUpgrade;
        [SerializeField] Mode _mode;

        public override bool CanUpgrade
        {
            get
            {
                if (IsMaxUpgraded)
                {
                    return false;
                }
                foreach (var keyValuePair in _costForCurrency)
                {
                    int cost = GetCost(keyValuePair.Value);
                    long currency = keyValuePair.Key;
                    if (currency < cost)
                    {
                        return false;
                    }
                }
                foreach (var kv in _intToUpgrade)
                {
                    int cost = GetCost(kv.Value);
                    int currency = kv.Key;
                    if (currency < cost)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override bool IsMaxUpgraded => _level >= _maxLevel;

        public override event UnityAction Upgraded;

        [ContextMenu("Upgrade")]
        public override void Upgrade()
        {
            if (CanUpgrade == false) return;
            foreach (var keyValuePair in _costForCurrency)
            {
                int cost = GetCost(keyValuePair.Value);
                Currency currency = keyValuePair.Key;
                currency.Value -= cost;
            }

            foreach (var keyValuePair in _intToUpgrade)
            {
                int cost = GetCost(keyValuePair.Value);
                Int currency = keyValuePair.Key;
                currency.Value -= cost;
            }

            PerformUpgrade();
            Upgraded?.Invoke();
        }
        public void ForceUpgrade()
        {
            if (IsMaxUpgraded)
            {
                return;
            }

            PerformUpgrade();
            Upgraded?.Invoke();
        }
        protected override void PerformUpgrade()
        {
            _level.Value++;
        }

        public Currency[] Currencys
        {
            get
            {
                List<Currency> result = new();

                result.AddRange(_costForCurrency.Keys);

                return result.ToArray();
            }
        }

        public Int[] IntsCosts 
        {
            get 
            {
                List<Int> result = new();
                result.AddRange(_intToUpgrade.Keys);
                return result.ToArray();
            }
        }

        private int GetCost(Int value)
        {
            if (value is IParametr<int> && _mode == Mode.PropertyByLevel)
            {
                IParametr<int> property = value as IParametr<int>;
                return property[_level];
            }

            return value;
        }

        private enum Mode
        {
            Simple = 0,
            PropertyByLevel = 1
        }

        protected override void OnValidate()
        {
            foreach(var cost in _costForCurrency.Values)
            {
                if(cost == null)
                {
                    Debug.LogError("Cost is null", this);
                }
            }
            foreach (var cost in _intToUpgrade.Values)
            {
                if (cost == null)
                {
                    Debug.LogError("Cost is null", this);
                }
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(MultyCurrencyUpgrader))]
        public class MultyCurrencyUpgraderEditor : Editor
        {
            private MultyCurrencyUpgrader _target;

            private void Awake()
            {
                _target = (MultyCurrencyUpgrader)target;
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (Application.IsPlaying(target) == false) { return; };
                EditorGUILayout.LabelField($"Can upgrade? {_target.CanUpgrade}");
                foreach (var keyValuePair in _target._costForCurrency)
                {
                    if (_target._mode == Mode.Simple)
                    {
                        EditorGUILayout.LabelField($"{keyValuePair.Key.name}: {keyValuePair.Value.Value} / {keyValuePair.Key.Value}");
                    }
                }
                foreach (var keyValuePair in _target._intToUpgrade)
                {
                    if (_target._mode == Mode.Simple)
                    {
                        EditorGUILayout.LabelField($"{keyValuePair.Key.name}: {keyValuePair.Value.Value} / {keyValuePair.Key.Value}");
                    }
                }
                if (_target.CanUpgrade && GUILayout.Button("Upgrade")) 
                {
                    _target.Upgrade();
                }
            }
        }
#endif
    }
}
