using System;

namespace UnityExtentions
{
    [Serializable]
    public struct Range
    {
        public float Min;
        public float Max;

        public float Middle => (Max + Min) / 2;
        public Range(float min = -1, float max = 1)
        {
            Min = min;
            Max = max;
        }


        public static implicit operator float(Range range)
        {
            return range.RandomValue();
        }


		public static implicit operator int(Range range)
		{
			return range.RandomIntValue();
		}

		public override string ToString()
		{
            return RandomValue().ToString();
		}
        public int RandomIntValue()
        {
            return UnityEngine.Random.Range((int)Min, (int)Max + 1);
        }
		public float RandomValue()
		{
            return UnityEngine.Random.Range(Min, Max);
		}

        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }
    }
}
