using System;
using UnityEngine;

namespace UnityExtentions.Random
{
    [Serializable]
    public class PercentageItem<T>
    {
        [SerializeField] private T _value;
        [Range(0, 1f)][SerializeField] private float _percentage;
        internal float Percentage => _percentage;
        internal T Value => _value;
        public PercentageItem()
        { }

        public PercentageItem(T item, float percentage)
        {
            _value = item;
            _percentage = Mathf.Clamp01(percentage);
        }
    }
}