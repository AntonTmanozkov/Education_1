using System;
using UnityEngine;

namespace SOVa.CommonVariables
{
    [CreateAssetMenu(menuName = "SOVa/Common variables/Long")]
    public class Long : VariableBase<long, LongChangedEvent>
    {
        protected override long ClampValue(long value)
        {
            return Math.Clamp(value, (long)_min.ObjValue, (long)_max.ObjValue);
        }
    }
}