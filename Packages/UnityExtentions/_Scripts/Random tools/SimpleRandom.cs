using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtentions.Random
{
    [Serializable]
    public class SimpleRandom<T>
    {
        [SerializeField] private List<T> _values = new();

        public T[] Values => _values.ToArray();
        public T RandomValue => _values.RandomElement();

        public SimpleRandom() 
        {

        }

        public SimpleRandom(SimpleRandom<T> source) 
        {
            _values = new(source._values);
        }

        public static implicit operator T(SimpleRandom<T> @object)
        {
            return @object.RandomValue;
        }
    }
}
