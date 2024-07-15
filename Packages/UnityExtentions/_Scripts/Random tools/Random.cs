using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtentions.Random
{ 
    [Serializable]
    public class Random<T>
    {
        [SerializeField] private List<ObjectInfo> _gameObjects = new();

        private List<ObjectInfo> _objectList;

        public List<ObjectInfo> Values 
        {
            get => _gameObjects;
        }

        public Random()
        {
            
        }

        public Random(Random<T> source)
        {
            _gameObjects = source._gameObjects;
            RegenerateList();
        }

        public static implicit operator T(Random<T> @object)
        {
            return @object.GetRandomObject();
        }

        public T GetRandomObject()
        {
            CheckList();
            int randomValue = UnityEngine.Random.Range(0, _objectList.Count);
            T result = _objectList[randomValue].Object;
            ObjectInfo droppedInfo = _objectList[randomValue];
            droppedInfo.Weight += droppedInfo.WeightOffcetWhenDropped;

            foreach (ObjectInfo info in _gameObjects)
            {
                if (info == droppedInfo) continue;

                info.Weight += info.WeightOffcetWhenDroppedOther;
            }

            RegenerateList();
            return result;
        }

        private void CheckList()
        {
            if (_objectList != null)
            {
                return;
            }
            RegenerateList();
        }

        private void RegenerateList()
        {
            _objectList = new List<ObjectInfo>();
            foreach (ObjectInfo info in _gameObjects)
            {
                float wight = info.Weight;
                while (wight > 0)
                {
                    wight -= 1;
                    _objectList.Add(info);
                }
            }
        }

        private float GetWeightSum()
        {
            float result = 0;

            foreach (ObjectInfo info in _gameObjects)
            {
                result += info.Weight;
            }

            return result;
        }

        [Serializable]
        public class ObjectInfo
        {
            public T Object;
            public int Weight = 1;
            public int WeightOffcetWhenDropped = 0;
            public int WeightOffcetWhenDroppedOther = 0;
            public float Chance;
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            RegenerateList();

            float sum = GetWeightSum();
            foreach (ObjectInfo info in _gameObjects)
            {
                info.Chance = info.Weight / sum;
            }
        }

#endif
    }
}