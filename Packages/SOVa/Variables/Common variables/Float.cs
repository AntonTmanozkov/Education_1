using System;
using UnityEngine;

namespace SOVa.CommonVariables
{
    [CreateAssetMenu(menuName = "SOVa/Common variables/Float")]
    public class Float : VariableBase<float, FloatChangedEvent>
    {
        protected override float ClampValue(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
    }
}
