using MemorySaveSystem;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SOVa
{
    public class VariableBase<T, TChangeEvent> : Variable, ISerializationCallbackReceiver where TChangeEvent : Event<T>
    {
        public static implicit operator T(VariableBase<T, TChangeEvent> reference) => reference.Value;

        [SerializeField] private T _value;
        [SerializeField] private bool _clamp;
        [SerializeField] protected Variable _min;
        [SerializeField] protected Variable _max;
        [SerializeField] private string _saveKey;
        [SerializeField] private bool _selfSave;
        [SerializeField] Event _changedEvent;
        [SerializeField] Variable _oldValue;
        private bool _isLoaded;
        private T _runtimeValue;

        public override Event Changed => _changedEvent;
        public override bool HaveOldValue => IsOldValue == false;
        public Event<T> TChanged => _changedEvent as Event<T>;

        public bool IsClamped => _clamp;
        public virtual T Min
        {
            get => (T)_min.ObjValue;
            set => _min.ObjValue = value;
        }
        public virtual T Max
        {
            get => (T)_max.ObjValue;
            set => _max.ObjValue = value;
        }

        public override string ToString()
        {
            if (Application.isPlaying) 
            {
                return _runtimeValue.ToString();
            }
            return _value.ToString();
        }

        public virtual T OldValue
        {
            get
            {
                if (IsOldValue)
                {
                    throw new OldValueException("Old value do not have old value!");
                }

                return (T)_oldValue.ObjValue;
            }
            protected set
            {
                if (IsOldValue)
                {
                    throw new OldValueException("Old value do not have old value!");
                }

                _oldValue.SetValue(value);
                if (LogChanges) 
                {
                    Debug.Log($"<color=yellow>{name}:</color> Value changed on: {_runtimeValue}", this);
                }
            }
        }

        public override object OldObjValue
        {
            get => OldValue;
            set => OldValue = (T)value;
        }

        public override object ObjValue
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (T)value;
            }
        }

        public virtual T Value
        {
            get
            {
                if (Application.isPlaying) 
                {
                    if (_isLoaded == false)
                    {
                        Load();
                    }
                    return _runtimeValue;
                }
                else 
                {
                    return _value;
                }
            }
            set
            {
                if (IsOldValue) 
                {
                    throw new OldValueException("Old value is read only!");
                }

                if (_isLoaded == false)
                {
                    Load();
                }

                if (_clamp)
                {
                    value = ClampValue(value);
                }
                _oldValue.SetValue(_runtimeValue);
                _runtimeValue = value;
                TChanged.Invoke(_runtimeValue);

                if (LogChanges) 
                {
                    Debug.Log($"<color=yellow>{name}:</color> Value changed: {_runtimeValue}", this);
                }
            }
        }

        protected virtual T ClampValue(T value)
        {
            return value;
        }
        protected override void OnEnable()
        {
            _isLoaded = false;
            base.OnEnable();
#if UNITY_EDITOR
            if (Application.IsPlaying(this) == false)
            {
                EditorApplication.playModeStateChanged += Check;
                return;
            }
#endif
            Load();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= Check;
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if UNITY_EDITOR
            List<Object> assetsToDestroy = new();
            if (_clamp) 
            {
                assetsToDestroy.Add(_min);
                assetsToDestroy.Add(_max);
                AssetDatabase.RemoveObjectFromAsset(_max);
                AssetDatabase.RemoveObjectFromAsset(_min);
                _min = null;
                _max = null;
            }

            if (_changedEvent != null) 
            {
                assetsToDestroy.Add(_changedEvent);
                AssetDatabase.RemoveObjectFromAsset(_changedEvent);
                _changedEvent = null;
            }

            if (_oldValue != null) 
            {
                VariableBase<T, TChangeEvent> oldValue = (VariableBase<T, TChangeEvent>)_oldValue;
                oldValue.OnDestroy();
                AssetDatabase.RemoveObjectFromAsset(oldValue);
            }
#endif
        }

#if UNITY_EDITOR
        private void Check(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                Load();
            }
        }
#endif

        protected virtual void Load()
        {
            if (_isLoaded)
            {
                return;
            }
            
            if (IsOldValue == false && HaveOldValue) 
            {
                #if UNITY_EDITOR
                if (_value == null || _oldValue == null)
                {
                    Debug.LogError($"You didn't pass value to {name}!", this);
                }
                else
                {
                    _oldValue.SetValue(_value);
                }
                #else
                _oldValue.SetValue(_value);
                #endif
            }
            _runtimeValue = _value;
            
            if (LogChanges)
            {
                Debug.Log($"<color=yellow>{name}:</color> Loaded from default: {_runtimeValue}", this);
            }

            _isLoaded = true;

            if (_selfSave) 
            {
                Memory.BeforeSaved += DoSave;
                Memory.Loaded += OnLoad;

                OnLoad();
            }
        }

        private void OnLoad()
        {
            SaveData data = new Load<SaveData>(_saveKey);
            if (data != null)
            {
                _runtimeValue = data.Value;
                _oldValue.SetValue(_runtimeValue);
            }

            TChanged.Invoke(Value);

            if (LogChanges)
            {
                Debug.Log($"<color=yellow>{name}:</color> Value after load: {_runtimeValue}", this);
            }
        }

        protected void InvokeChanged()
        {
            if (HaveChangedEvent)
                Changed.Invoke();
        }
        private void DoSave()
        {
            SaveData data = new()
            {
                Value = _runtimeValue,
            };
            new Save(_saveKey, data);
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
        }

        [Serializable]
        private class SaveData
        {
            public T Value;
        }

        internal override void SetValue(object value)
        {
            T parsedValue = (T)value;
            _runtimeValue = parsedValue;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.IsPlaying(this))
            {
                Changed?.Invoke();
            }
            ValidateClamp();
        }

        private void ValidateClamp() 
        {
            if (_clamp)
            {
                Type minMaxType = GetType();
                if (_max == null)
                {
                    _max = CreateInstance(minMaxType) as Variable;
                    _max.name = $"{name}.Max";
                    AssetDatabase.AddObjectToAsset(_max, this);
                }
                if (_min == null)
                {
                    _min = CreateInstance(minMaxType) as Variable;
                    _min.name = $"{name}.Min";
                    AssetDatabase.AddObjectToAsset(_min, this);
                }
            }
            else
            {
                if (_max != null)
                {
                    VariableBase<T, TChangeEvent> maxValue = _max as VariableBase<T, TChangeEvent>;

                    AssetDatabase.RemoveObjectFromAsset(maxValue);
                    AssetDatabase.RemoveObjectFromAsset(maxValue.Changed);
                    AssetDatabase.RemoveObjectFromAsset(maxValue._oldValue);
                    DestroyImmediate(maxValue.Changed);
                    DestroyImmediate(maxValue._oldValue);
                    DestroyImmediate(maxValue);
                    _max = null;
                }
                if (_min != null)
                {
                    VariableBase<T, TChangeEvent> minValue = _min as VariableBase<T, TChangeEvent>;

                    AssetDatabase.RemoveObjectFromAsset(minValue);
                    AssetDatabase.RemoveObjectFromAsset(minValue.Changed);
                    AssetDatabase.RemoveObjectFromAsset(minValue._oldValue);
                    DestroyImmediate(minValue.Changed);
                    DestroyImmediate(minValue._oldValue);
                    DestroyImmediate(minValue);
                    _min = null;
                }
            }
        }

        protected override void AfterCreate_EDITOR()
        {
            Construct();
        }

        protected override void PerformRename_EDITOR()
        {
            bool dirty = false;
            if (HaveOldValue && _oldValue != null && _oldValue.name != $"{name}.OldValue")
            {
                _oldValue.name = $"{name}.OldValue";
                dirty = true;
            }
            if (HaveChangedEvent && _changedEvent.name != $"{name}.Changed")
            {
                _changedEvent.name = $"{name}.Changed";
                dirty = true;
            }
            if (_clamp && (_max.name != $"{name}.Max" || _min.name != $"{name}.Min"))
            {
                _max.name = $"{name}.Max";
                _min.name = $"{name}.Min";
                dirty = true;
            }
            if (dirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
                AssetDatabase.Refresh();
            }
        }

        private void Construct()
        {
            if (IsOldValue) { return; }

            if (_changedEvent == null && HaveChangedEvent)
            {
                Type eventType = typeof(TChangeEvent);
                _changedEvent = CreateInstance(eventType) as Event;
                _changedEvent.name = $"{name}.Changed";
                AssetDatabase.AddObjectToAsset(_changedEvent, this);
            }

            if (_oldValue == null && HaveOldValue) 
            {
                Type mainType = GetType();
                _oldValue = CreateInstance(mainType) as Variable;
                _oldValue.name = $"{name}.OldValue";
                _oldValue.IsOldValue = true;
                ((VariableBase<T, TChangeEvent>)_oldValue)._changedEvent = _changedEvent;
                AssetDatabase.AddObjectToAsset(_oldValue, this);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}
