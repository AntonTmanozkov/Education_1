using System;
using System.ComponentModel;
using UnityEngine;

namespace MemorySaveSystem
{
	public class Load<T>
	{
		private string _saveName;
		private T _data;

		public Load(string saveName)
		{
			_saveName = saveName;
		}

		public T Data 
		{
			get 
			{
                if (Memory.saves.Keys.Contains(_saveName) == false)
                {
                    _data = default;

                    if ((int)MemorySettings.Instance.DebugModeValue > 1)
                        Debug.Log($"{Memory.MESSAGE_PREFFIX} Loaded by key {_saveName}: {JsonUtility.ToJson(_data)}");

                    return _data;
                }
                Type dataType = typeof(T);
                if (dataType.IsPrimitive || dataType == typeof(decimal))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(dataType);
                    _data = (T)converter.ConvertFrom(Memory.saves[_saveName]);
                }
                else if(dataType == typeof(string))
                {
                    _data = (T)(object)Memory.saves[_saveName];
                }
                else
                {
                    _data = JsonUtility.FromJson<T>(Memory.saves[_saveName]);
                }

                if ((int)MemorySettings.Instance.DebugModeValue > 1)
                    Debug.Log($"{Memory.MESSAGE_PREFFIX} Loaded by key {_saveName}: {JsonUtility.ToJson(_data)}");

                return _data;
            }
		}

        public static implicit operator T(Load<T> load)
		{
            return load.Data;
		}
	}
}