using UnityEngine;

namespace SOVa
{
    public static class NumberCompare
    {
        
        public static bool Do(float first, float second, NumberCompareType type)
        {
            switch (type)
            {
                case NumberCompareType.Equal:
                    return Mathf.Approximately(first, second);
                case NumberCompareType.NotEqual:
                    return Mathf.Approximately(first, second) == false;
                case NumberCompareType.Greater:
                    return first > second;
                case NumberCompareType.GreaterOrEqual:
                    return first >= second;
                case NumberCompareType.Less:
                    return first < second;
                case NumberCompareType.LessOrEqual:
                    return first <= second;
                default:
                    Debug.LogError("Not known case");
                    return false;
            }
        }
        public static bool Do(int first, int second, NumberCompareType type)
        {
            switch (type)
            {
                case NumberCompareType.Equal:
                    return first == second;
                case NumberCompareType.NotEqual:
                    return first != second;
                case NumberCompareType.Greater:
                    return first > second;
                case NumberCompareType.GreaterOrEqual:
                    return first >= second;
                case NumberCompareType.Less:
                    return first < second;
                case NumberCompareType.LessOrEqual:
                    return first <= second;
                default:
                    Debug.LogError("Not known case");
                    return false;
            }
        }
    }
}