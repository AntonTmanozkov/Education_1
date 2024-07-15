# ������� ���������� Memory
Unity extentions - ����� ����������� ������������, ������� ����������� ������������.

## ����������
1. [Random](###Random)

---
### Random
�����, ������� ������������ ��� ���������� ������ �������� �� �����.
<br/> ��������� ������

```C#
namespace UnityExtentions
{
    [Serializable]
    public class Random<T>
    {
        [SerializeField] private List<ObjectInfo> _gameObjects = new();

        private List<ObjectInfo> _objectList;

        public List<ObjectInfo> Values;

        public Random();
        public Random(Random<T> source);

        public static implicit operator T(Random<T> @object);

        public T GetRandomObject();
        private void CheckList();

        private void RegenerateList();
        private float GetWeightSum();

        [Serializable]
        public class ObjectInfo
        {
            public T Object;
            public int Weight;
            public int WeightOffcetWhenDropped;
            public int WeightOffcetWhenDroppedOther;
            public float Chance;
        }

#if UNITY_EDITOR
        public void OnValidate();
#endif
    }
}
```