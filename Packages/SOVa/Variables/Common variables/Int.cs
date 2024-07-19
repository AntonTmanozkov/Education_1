using UnityEngine;

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Common variables/Int")]
    public class Int : VariableBase<int, IntChangedEvent>
    {
        protected override int ClampValue(int value)
        {
            return Mathf.Clamp(value, Min, Max);
        }

        //For unity events
        public void Add(int value) 
        {
            Value += value;
        }

        public void Remove(int value)
        {
            Value -= value;
        }
    }
}
