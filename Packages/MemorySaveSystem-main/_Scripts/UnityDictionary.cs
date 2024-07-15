using UnityEngine;
using System;
using System.Collections.Generic;

namespace MemorySaveSystem
{

    [Serializable]
    internal class MemoryFile<Key, Value>
    {
        public string SaveDate;
        public string SaveVersion;
        public string ProductName;
        [SerializeField] private List<KeyValue> _values = new();

        public MemoryFile(string saveDate) 
        {
            SaveDate = saveDate;
        }
        public MemoryFile(string saveDate, MemoryFile<Key, Value> source)
        {
            SaveDate = saveDate;
            _values = new(source._values);
        }

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        public List<Key> Keys
        {
            get
            {
                List<Key> result = new();

                foreach (KeyValue i in _values)
                {
                    result.Add(i.Key);
                }
                return result;
            }
        }

        public List<Value> Values
        {
            get
            {
                List<Value> result = new();

                foreach (KeyValue i in _values)
                {
                    result.Add(i.Value);
                }
                return result;
            }
        }

        public Dictionary<T1Key, T2Value> ToDictionary<T1Key, T2Value>() 
        {
            Dictionary<T1Key, T2Value> result = new();
            foreach (KeyValue pair in _values) 
            {
                result.Add((T1Key)(object)pair.Key, (T2Value)(object)pair.Value);
            }
            return result;
        }

        public bool IsReadOnly => false;

        public Value this[Key index]
        {
            get
            {
                foreach (KeyValue i in _values)
                {
                    if (i.Key == null)
                    {
                        continue;
                    }
                    if (i.Key.Equals(index))
                    {
                        return i.Value;
                    }
                }
                throw new ArgumentException($"Can't find {typeof(Value).Name} with {index} key in this dictionary");
            }
            set
            {
                for (int i = 0; i < _values.Count; i++)
                {
                    if (_values[i].Key.Equals(index))
                    {
                        _values[i].Value = value;
                    }
                }
            }
        }

        public void Add(Key key, Value value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            _values.Add(new KeyValue(key, value));
        }

        public void Remove(Key key)
        {
            foreach (KeyValue keyValue in _values.ToArray())
            {
                if (keyValue.Key.Equals(key))
                {
                    _values.Remove(keyValue);
                }
            }
        }

        public void Clear()
        {
            _values = new();
        }


        [Serializable]
        private class KeyValue
        {
            public Key Key;
            public Value Value;

            public KeyValue(Key key, Value value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}