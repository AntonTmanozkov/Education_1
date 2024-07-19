using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOVa.CommonVariables
{
    [Serializable]
    public class ParametrBase<T> : IEnumerable<T>
    {
        [SerializeField] private List<T> _parametrPerLevel = new();
        [SerializeField] private int _indexOffset = -1;
        
        public Int Level;
        public UnityEngine.Object owner;
        public int MaxPossibleLevel => _parametrPerLevel.Count;
        private string name => Level.name;

        public T Value
        {
            get => this[Level];
            set => Debug.LogWarning($"{name} is readonly!", owner);
        }

        public int CurrentIndex => Level;

        public T this[int index]
        {
            get
            {
                index = CheckIndex(index + _indexOffset);
                return _parametrPerLevel[index];
            }
            set
            {
                Debug.LogWarning($"{name} is readonly!", owner);
            }
        }

        public void Add(T value) 
        {
            _parametrPerLevel.Add(value);
        }

        public void Remove(T value) 
        {
            _parametrPerLevel.Remove(value);
        }

        private int CheckIndex(int index)
        {
            if (index >= 0 && index < _parametrPerLevel.Count)
            {
                return index;
            }
            else
            {
                Debug.LogError($"You tried to get value that is not possible to get from {owner.name}! " +
                    $"Last index is {MaxPossibleLevel - 1}, but you passed {index} index", owner);
                return _parametrPerLevel.Count - 1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var p in _parametrPerLevel)
            {
                yield return p;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); 
        }
    }
}
