using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtentions.Random
{
    [Serializable]
    public class PercentageItemList<T> : IEnumerable<T>

    {
        [SerializeField] private PercentageItem<T>[] _items; 
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] GetItems()
        {
            var chosen = new List<T>();
            foreach(var item in _items)
            {
                float randomPercentage = UnityEngine.Random.Range(0, 1f);
                if (randomPercentage <= item.Percentage)
                {
                    chosen.Add(item.Value);
                }
            }
            return chosen.ToArray();    
        }
    }
}